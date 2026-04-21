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
                    }
                ]
            },
            {
                path: 'assets',
                loadComponent: () => import('./pages/assets/assets.component').then(c => c.AssetsComponent),
                canActivate: [permissionGuard],
                data: { permissions: ['Assets.View'] }
            },
            {
                path: 'employees',
                loadComponent: () => import('./pages/employees/employees.component').then(c => c.EmployeesComponent),
                canActivate: [permissionGuard],
                data: { permissions: ['Employees.View'] }
            },
            {
                path: 'maintenances',
                loadComponent: () => import('./pages/maintenances/maintenances.component').then(c => c.MaintenancesComponent),
                canActivate: [permissionGuard],
                data: { permissions: ['Maintenances.View'] }
            },
            {
                path: 'documents',
                loadComponent: () => import('./pages/documents/documents.component').then(c => c.DocumentsComponent),
                canActivate: [permissionGuard],
                data: { permissions: ['Documents.View'] }
            },
            {
                path: 'labors',
                loadComponent: () => import('./pages/labors/labors.component').then(c => c.LaborsComponent),
                canActivate: [permissionGuard],
                data: { permissions: ['Labors.View'] }
            },
            { path: '', redirectTo: 'home', pathMatch: 'full' }
        ]
    },
    { path: '**', redirectTo: 'login' }
];
