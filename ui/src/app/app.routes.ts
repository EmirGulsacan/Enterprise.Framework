import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout.component';
import { authGuard } from './guards/auth.guard';
import { permissionGuard } from './guards/permission.guard';
export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: '',
        component: MainLayoutComponent,
        canActivate: [authGuard],
        children: [
            { 
                path: 'home', 
                loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) 
            },
            { 
                path: 'users', 
                loadComponent: () => import('./pages/users/user-management.component').then(m => m.UserManagementComponent),
                canActivate: [permissionGuard],
                data: { permissions: ['Identity.Users.View'] }
            },
            {
                path: 'management',
                children: [
                    {
                        path: 'roles',
                        loadComponent: () => import('./pages/management/roles/roles.component').then(m => m.RolesComponent),
                        canActivate: [permissionGuard],
                        data: { permissions: ['Identity.Roles.View'] }
                    },
                    {
                        path: 'rule-engine',
                        loadComponent: () => import('./features/rule-engine/rule-builder.component').then(m => m.RuleBuilderComponent)
                    }
                ]
            },
            { path: '', redirectTo: 'home', pathMatch: 'full' }
        ]
    },
    { path: '**', redirectTo: 'login' }
];
