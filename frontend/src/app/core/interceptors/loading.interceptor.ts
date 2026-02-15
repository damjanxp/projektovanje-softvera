import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { LoadingService } from '../services/loading.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  // Only show loading for mutating requests (POST, PUT, DELETE, PATCH)
  const showLoader = ['POST', 'PUT', 'DELETE', 'PATCH'].includes(req.method);

  if (showLoader) {
    loadingService.show();
  }

  return next(req).pipe(
    finalize(() => {
      if (showLoader) {
        loadingService.hide();
      }
    })
  );
};
