import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  const token = authService.getToken();
  
  // Clone the request and add authorization header if token exists
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle 401 Unauthorized
      if (error.status === 401) {
        console.error('Unauthorized request - logging out');
        authService.logout();
        router.navigate(['/login']);
      }
      
      // Handle 403 Forbidden
      if (error.status === 403) {
        console.error('Forbidden - insufficient permissions');
      }

      // Log the error for debugging
      console.error('HTTP Error:', {
        status: error.status,
        message: error.message,
        url: error.url
      });

      return throwError(() => error);
    })
  );
};
