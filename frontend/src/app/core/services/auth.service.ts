import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, AuthResponse } from '../../shared/models';

export type UserRole = 'Tourist' | 'Guide' | 'Admin';

interface JwtPayload {
  sub?: string;
  nameid?: string;
  unique_name?: string;
  role?: string;
  exp?: number;
  [key: string]: unknown;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';
  
  private http = inject(HttpClient);
  private router = inject(Router);

  login(username: string, password: string): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${environment.apiUrl}/auth/login`, {
      username,
      password
    }).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setToken(response.data.token);
          this.setUser(response.data);
        }
      }),
      catchError(error => {
        console.error('Login error:', error);
        const errorMessage = error.error?.error?.message 
          || error.error?.message 
          || 'An error occurred during login';
        return of({
          success: false,
          error: {
            code: 'LOGIN_ERROR',
            message: errorMessage
          }
        } as ApiResponse<AuthResponse>);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    // Check if token is expired using JWT payload
    const payload = this.decodeJwtPayload(token);
    if (payload?.exp) {
      const expiresAt = new Date(payload.exp * 1000);
      if (expiresAt <= new Date()) {
        this.logout();
        return false;
      }
    }

    return true;
  }

  getUsername(): string | null {
    // First try from stored user data
    const user = this.getUser();
    if (user?.username) {
      return user.username;
    }

    // Fallback to JWT claims
    const token = this.getToken();
    if (token) {
      const payload = this.decodeJwtPayload(token);
      return payload?.unique_name || payload?.sub || null;
    }

    return null;
  }

  getUserId(): string | null {
    const user = this.getUser();
    if (user?.userId) {
      return user.userId;
    }

    // Fallback to JWT claims
    const token = this.getToken();
    if (token) {
      const payload = this.decodeJwtPayload(token);
      return payload?.nameid || payload?.sub || null;
    }

    return null;
  }

  getUserRole(): UserRole | null {
    // First try from stored user data
    const user = this.getUser();
    if (user?.role) {
      return user.role as UserRole;
    }

    // Fallback to JWT claims
    const token = this.getToken();
    if (token) {
      const payload = this.decodeJwtPayload(token);
      const role = payload?.role || payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      if (role && this.isValidRole(role as string)) {
        return role as UserRole;
      }
    }

    return null;
  }

  hasRole(allowedRoles: UserRole[]): boolean {
    const userRole = this.getUserRole();
    if (!userRole) {
      return false;
    }
    return allowedRoles.includes(userRole);
  }

  private isValidRole(role: string): role is UserRole {
    return ['Tourist', 'Guide', 'Admin'].includes(role);
  }

  private decodeJwtPayload(token: string): JwtPayload | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) {
        return null;
      }

      const payload = parts[1];
      // Base64Url decode
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const paddedBase64 = base64 + '=='.slice(0, (4 - base64.length % 4) % 4);
      const jsonPayload = decodeURIComponent(
        atob(paddedBase64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );

      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error('Error decoding JWT:', error);
      return null;
    }
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private setUser(user: AuthResponse): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  private getUser(): AuthResponse | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (!userJson) {
      return null;
    }
    try {
      return JSON.parse(userJson);
    } catch {
      return null;
    }
  }
}
