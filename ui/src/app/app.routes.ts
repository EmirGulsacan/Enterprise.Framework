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
                        path: 'organization',
                        loadComponent: () => import('./pages/management/organization/organization.component').then(m => m.OrganizationComponent),
                        canActivate: [permissionGuard],
                        data: { permissions: ['Locations.View'] }
                    }
                ]
            },
            {
                path: 'assets',
                loadComponent: () => import('./pages/assets/assets.component').then(c => c.AssetsComponent)
            },
            {
                path: 'employees',
                loadComponent: () => import('./pages/employees/employees.component').then(c => c.EmployeesComponent)
            },
            { path: '', redirectTo: 'home', pathMatch: 'full' }
        ]
    },
    { path: '**', redirectTo: 'login' }
];
