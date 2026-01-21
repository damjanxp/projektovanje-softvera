import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService, UserRole } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // First check if user is authenticated
  if (!authService.isAuthenticated()) {
    router.navigate(['/login'], { 
      queryParams: { returnUrl: state.url } 
    });
    return false;
  }

  // Get allowed roles from route data
  const allowedRoles = route.data['roles'] as UserRole[] | undefined;
  
  // If no roles specified, just require authentication
  if (!allowedRoles || allowedRoles.length === 0) {
    return true;
  }

  // Check if user has one of the allowed roles
  if (authService.hasRole(allowedRoles)) {
    return true;
  }

  // User doesn't have required role - redirect to forbidden
  router.navigate(['/forbidden']);
  return false;
};
