import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, Notification } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-container">
      @for (notification of notifications$ | async; track notification.id) {
        <div class="notification" [class]="'notification-' + notification.type">
          <span class="notification-message">{{ notification.message }}</span>
          <button class="notification-close" (click)="dismiss(notification.id)">&times;</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .notification-container {
      position: fixed;
      top: 70px;
      right: 20px;
      z-index: 9998;
      display: flex;
      flex-direction: column;
      gap: 10px;
      max-width: 400px;
    }

    .notification {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 12px 16px;
      border-radius: 4px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
      animation: slideIn 0.3s ease-out;
    }

    @keyframes slideIn {
      from {
        transform: translateX(100%);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }

    .notification-success {
      background-color: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }

    .notification-error {
      background-color: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .notification-info {
      background-color: #d1ecf1;
      color: #0c5460;
      border: 1px solid #bee5eb;
    }

    .notification-message {
      flex: 1;
      margin-right: 12px;
    }

    .notification-close {
      background: none;
      border: none;
      font-size: 20px;
      cursor: pointer;
      opacity: 0.7;
      padding: 0;
      line-height: 1;
    }

    .notification-close:hover {
      opacity: 1;
    }
  `]
})
export class NotificationComponent {
  private notificationService = inject(NotificationService);

  notifications$ = this.notificationService.notifications$;

  dismiss(id: number): void {
    this.notificationService.dismiss(id);
  }
}
