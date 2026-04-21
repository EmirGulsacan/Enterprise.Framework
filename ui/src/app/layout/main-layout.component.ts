import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../services/auth.service';
@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule],
  template: `
    <div class="layout-wrapper">
      <div class="layout-sidebar">
        <div class="flex align-items-center gap-3 p-4 mb-3">
          <div class="bg-accent flex align-items-center justify-content-center border-round-xl shadow-4" style="width: 42px; height: 42px; background: var(--accent-color)">
            <i class="pi pi-shield text-white text-xl"></i>
          </div>
          <div>
            <div class="text-white font-bold text-xl tracking-tight">Enterprise.Framework</div>
            <div class="text-white-alpha-60 text-xs font-medium">Enterprise Portal</div>
          </div>
        </div>
        <nav class="px-3 mt-4">
          <div class="text-white-alpha-40 text-xs font-bold mb-3 px-3 uppercase tracking-widest">Menü</div>
          <ul class="list-none p-0 m-0 flex flex-column gap-2">
            <li>
              <a routerLink="/home" routerLinkActive="active" class="nav-link">
                <i class="pi pi-home"></i>
                <span>Anasayfa</span>
              </a>
            </li>
            <li *ngIf="authService.hasPermission('Employees.View')">
              <a routerLink="/employees" routerLinkActive="active" class="nav-link">
                <i class="pi pi-id-card"></i>
                <span>Çalışan Yönetimi</span>
              </a>
            </li>
            <li *ngIf="authService.hasPermission('Assets.View')">
              <a routerLink="/assets" routerLinkActive="active" class="nav-link">
                <i class="pi pi-box"></i>
                <span>Varlık Yönetimi</span>
              </a>
            </li>
            <li *ngIf="authService.hasPermission('Maintenances.View')">
              <a routerLink="/maintenances" routerLinkActive="active" class="nav-link">
                <i class="pi pi-wrench"></i>
                <span>Bakım Yönetimi</span>
              </a>
            </li>
            <li *ngIf="authService.hasPermission('Documents.View')">
              <a routerLink="/documents" routerLinkActive="active" class="nav-link">
                <i class="pi pi-file"></i>
                <span>Doküman Yönetimi</span>
              </a>
            </li>
            <li *ngIf="authService.hasPermission('Labors.View')">
              <a routerLink="/labors" routerLinkActive="active" class="nav-link">
                <i class="pi pi-cog"></i>
                <span>İş Gücü Yönetimi</span>
              </a>
            </li>
          </ul>
          <div class="text-white-alpha-40 text-xs font-bold mb-3 px-3 mt-4 uppercase tracking-widest" *ngIf="authService.hasPermission('Identity.Users.View') || authService.hasPermission('Identity.Roles.View')">Yönetim</div>
          <ul class="list-none p-0 m-0 flex flex-column gap-2">
            <li *ngIf="authService.hasPermission('Identity.Users.View')">
              <a routerLink="/users" routerLinkActive="active" class="nav-link">
                <i class="pi pi-users"></i>
                <span>Kullanıcı Yönetimi</span>
              </a>
            </li>
            <li *ngIf="authService.hasPermission('Identity.Roles.View')">
              <a routerLink="/management/roles" routerLinkActive="active" class="nav-link">
                <i class="pi pi-lock"></i>
                <span>Rol ve Yetkiler</span>
              </a>
            </li>
          </ul>
        </nav>
        <div class="mt-auto p-4">
            <div class="bg-primary-light border-round-xl p-3 flex align-items-center gap-3">
                <div class="w-2rem h-2rem border-circle bg-accent flex align-items-center justify-content-center text-white font-bold" style="background: var(--accent-color)">
                    {{ authService.username?.charAt(0)?.toUpperCase() }}
                </div>
                <div class="flex flex-column overflow-hidden">
                    <span class="text-white text-sm font-bold truncate">{{ authService.username }}</span>
                    <span class="text-white-alpha-50 text-xs truncate">Sistem Yöneticisi</span>
                </div>
            </div>
        </div>
      </div>
      <div class="layout-container">
        <header class="layout-topbar">
          <div class="flex align-items-center gap-3">
            <button pButton icon="pi pi-bars" class="p-button-text p-button-rounded text-600 lg:hidden"></button>
            <h2 class="text-xl font-bold text-900 m-0">Hoş Geldin, <span class="text-accent" style="color: var(--accent-color)">{{ authService.username }}</span></h2>
          </div>
          <div class="flex align-items-center gap-4">
            <div class="hidden md:flex align-items-center p-input-icon-left">
                <i class="pi pi-search text-500"></i>
                <input type="text" pInputText placeholder="Ara..." class="p-inputtext-sm border-round-xl border-none bg-slate-100" style="background: #f1f5f9; width: 250px" />
            </div>
            <div class="flex align-items-center gap-2">
                <button pButton icon="pi pi-bell" class="p-button-text p-button-rounded text-600 relative">
                    <span class="absolute top-0 right-0 w-8px h-8px bg-red-500 border-circle border-2 border-white" style="width: 10px; height: 10px; right: 5px; top: 5px"></span>
                </button>
                <button pButton icon="pi pi-sign-out" (click)="authService.logout()" class="p-button-text p-button-rounded p-button-danger"></button>
            </div>
          </div>
        </header>
        <main class="layout-content">
          <div class="fadein animation-duration-500">
            <router-outlet></router-outlet>
          </div>
        </main>
      </div>
    </div>
  `
})
export class MainLayoutComponent {
  constructor(public authService: AuthService) {}
}
