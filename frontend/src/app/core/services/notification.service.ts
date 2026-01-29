import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ApiResponse } from '../../shared/models';

export type NotificationType = 'success' | 'error' | 'info';

export interface Notification {
  id: number;
  type: NotificationType;
  message: string;
  autoDismiss: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  private nextId = 0;
  private readonly DEFAULT_TIMEOUT = 5000;

  notifications$: Observable<Notification[]> = this.notificationsSubject.asObservable();

  success(message: string, autoDismiss = true): void {
    this.addNotification('success', message, autoDismiss);
  }

  error(message: string, autoDismiss = true): void {
    this.addNotification('error', message, autoDismiss);
  }

  info(message: string, autoDismiss = true): void {
    this.addNotification('info', message, autoDismiss);
  }

  dismiss(id: number): void {
    const current = this.notificationsSubject.value;
    this.notificationsSubject.next(current.filter(n => n.id !== id));
  }

  clearAll(): void {
    this.notificationsSubject.next([]);
  }

  /**
   * Parse error from ApiResponse or HttpErrorResponse
   */
  parseError(error: unknown): string {
    // Handle ApiResponse error
    if (this.isApiResponse(error)) {
      return error.error?.message || 'An unexpected error occurred';
    }

    // Handle HttpErrorResponse
    if (error instanceof HttpErrorResponse) {
      // Try to extract error message from response body
      if (error.error?.error?.message) {
        return error.error.error.message;
      }
      if (error.error?.message) {
        return error.error.message;
      }
      
      // Handle common HTTP status codes
      switch (error.status) {
        case 0:
          return 'Unable to connect to server. Please check your connection.';
        case 400:
          return 'Invalid request. Please check your input.';
        case 401:
          return 'Session expired. Please log in again.';
        case 403:
          return 'You do not have permission to perform this action.';
        case 404:
          return 'The requested resource was not found.';
        case 500:
          return 'Server error. Please try again later.';
        default:
          return error.message || 'An unexpected error occurred';
      }
    }

    // Handle generic Error
    if (error instanceof Error) {
      return error.message;
    }

    // Handle string error
    if (typeof error === 'string') {
      return error;
    }

    return 'An unexpected error occurred';
  }

  /**
   * Show error notification from ApiResponse or HttpErrorResponse
   */
  showError(error: unknown): void {
    const message = this.parseError(error);
    this.error(message);
  }

  private addNotification(type: NotificationType, message: string, autoDismiss: boolean): void {
    const notification: Notification = {
      id: this.nextId++,
      type,
      message,
      autoDismiss
    };

    const current = this.notificationsSubject.value;
    this.notificationsSubject.next([...current, notification]);

    if (autoDismiss) {
      setTimeout(() => {
        this.dismiss(notification.id);
      }, this.DEFAULT_TIMEOUT);
    }
  }

  private isApiResponse(obj: unknown): obj is ApiResponse<unknown> {
    return typeof obj === 'object' && obj !== null && 'success' in obj;
  }
}
