import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';
export const permissionGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const notification = inject(NotificationService);
  const requiredPermissions = route.data['permissions'] as string[];
  if (!requiredPermissions || requiredPermissions.length === 0) {
    return true;
  }
  const hasPermission = requiredPermissions.every(p => authService.hasPermission(p));
  if (hasPermission) {
    return true;
  }
  notification.error('Bu sayfaya erişim yetkiniz bulunmamaktadır.');
  router.navigate(['/']);
  return false;
};
