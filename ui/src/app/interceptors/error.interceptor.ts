import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '../services/notification.service';
import { catchError, throwError } from 'rxjs';
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Beklenmeyen bir hata oluştu.';
      if (error.error instanceof ErrorEvent) {
        errorMessage = error.error.message;
      } else {
        switch (error.status) {
          case 401:
            errorMessage = 'Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.';
            const router = inject(Router);
            if (!router.url.includes('/login')) {
              router.navigate(['/login']);
            }
            break;
          case 403:
            errorMessage = 'Bu işlem için yetkiniz bulunmamaktadır.';
            break;
          case 404:
            errorMessage = 'İstenilen kaynak bulunamadı.';
            break;
          case 422: 
            if (error.error?.errors && error.error.errors.length > 0) {
              errorMessage = error.error.errors.join(' ');
            } else {
              errorMessage = error.error?.message || 'İş kuralı ihlali.';
            }
            break;
          case 400: 
            if (error.error?.errors && error.error.errors.length > 0) {
              errorMessage = error.error.errors.join(' ');
            } else {
              errorMessage = error.error?.message || 'Geçersiz istek.';
            }
            break;
          case 500:
            errorMessage = 'Sunucu tarafında bir hata oluştu.';
            break;
          default:
            if (error.error?.errors && error.error.errors.length > 0) {
                errorMessage = error.error.errors.join(' ');
            } else {
                errorMessage = error.error?.message || errorMessage;
            }
            break;
        }
      }
      if (error.error?.traceId) {
        errorMessage += ` (Hata No: ${error.error.traceId})`;
      }
      notificationService.error(errorMessage);
      return throwError(() => error);
    })
  );
};
