import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageService } from 'primeng/api';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, InputTextModule, PasswordModule, ButtonModule, CardModule, InputGroupModule, InputGroupAddonModule],
  template: `
    <div class="login-wrapper">
      <div class="login-bg-shapes">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
      </div>
      <div class="login-card shadow-8">
        <div class="login-header">
          <div class="logo-container">
            <i class="pi pi-shield text-4xl text-primary"></i>
          </div>
          <h1 class="text-900 text-3xl font-bold mt-4 mb-2">Enterprise.Framework PORTAL</h1>
          <p class="text-600 font-medium">Lütfen kimlik bilgilerinizi giriniz</p>
        </div>
        <div class="login-body">
          <div class="p-inputgroup mb-4">
            <span class="p-inputgroup-addon bg-white border-right-none">
              <i class="pi pi-user text-600"></i>
            </span>
            <input type="text" pInputText [(ngModel)]="username" placeholder="Kullanıcı Adı" class="border-left-none" (keyup.enter)="login()" />
          </div>
          <div class="p-inputgroup mb-5">
            <span class="p-inputgroup-addon bg-white border-right-none">
              <i class="pi pi-lock text-600"></i>
            </span>
            <p-password [(ngModel)]="password" [feedback]="false" [toggleMask]="true" placeholder="Şifre" styleClass="w-full" inputStyleClass="border-left-none" (keyup.enter)="login()"></p-password>
          </div>
          <p-button label="GİRİŞ YAP" (onClick)="login()" [loading]="loading" styleClass="w-full p-3 text-lg font-bold border-round-lg shadow-2"></p-button>
          <div class="mt-4 text-center">
            <a class="text-primary font-bold cursor-pointer hover:underline">Şifremi Unuttum?</a>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-wrapper {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background: #0f172a;
      display: flex;
      align-items: center;
      justify-content: center;
      overflow: hidden;
      z-index: 9999;
    }
    .login-bg-shapes {
      position: absolute;
      width: 100%;
      height: 100%;
      z-index: 1;
      overflow: hidden;
    }
    .shape {
      position: absolute;
      border-radius: 50%;
      filter: blur(80px);
      opacity: 0.4;
    }
    .shape-1 {
      width: 400px;
      height: 400px;
      background: #20a17f;
      top: -100px;
      right: -100px;
    }
    .shape-2 {
      width: 500px;
      height: 500px;
      background: #3b82f6;
      bottom: -150px;
      left: -150px;
    }
    .login-card {
      width: 450px;
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(10px);
      border-radius: 24px;
      padding: 3rem;
      z-index: 2;
      border: 1px solid rgba(255, 255, 255, 0.3);
      animation: fadeInScale 0.5s ease-out;
    }
    .login-header {
      text-align: center;
      margin-bottom: 2.5rem;
    }
    .logo-container {
      background: #f1f5f9;
      width: 80px;
      height: 80px;
      border-radius: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      margin: 0 auto;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    }
    @keyframes fadeInScale {
      from { opacity: 0; transform: scale(0.95); }
      to { opacity: 1; transform: scale(1); }
    }
    .login-body {
        .p-inputgroup {
            .p-inputgroup-addon {
                border-top-left-radius: 12px;
                border-bottom-left-radius: 12px;
                padding-left: 1.25rem;
                padding-right: 1rem;
                border: 1px solid #d1d5db;
                border-right: none;
            }
            .p-inputtext, input {
                border-top-right-radius: 12px;
                border-bottom-right-radius: 12px;
                height: 52px;
                border: 1px solid #d1d5db;
                border-left: none;
                &:focus {
                    box-shadow: none;
                    border-color: #3b82f6;
                }
            }
        }
    }
    :host ::ng-deep .p-password {
        width: 100%;
        input {
            border-top-right-radius: 12px !important;
            border-bottom-right-radius: 12px !important;
            border-top-left-radius: 0 !important;
            border-bottom-left-radius: 0 !important;
            height: 52px;
        }
    }
  `]
})
export class LoginComponent {
  username = '';
  password = '';
  loading = false;
  constructor(
    private authService: AuthService,
    private router: Router,
    private messageService: MessageService
  ) {}
  async login() {
    if (!this.username || !this.password) return;
    this.loading = true;
    try {
      const success = await this.authService.loginWithCredentials(this.username, this.password);
      if (success) {
        this.router.navigate(['/']);
      } else {
        this.messageService.add({severity:'error', summary:'Hata', detail:'Kullanıcı adı veya şifre hatalı'});
      }
    } catch (error) {
      this.messageService.add({severity:'error', summary:'Hata', detail:'Giriş yapılamadı'});
    } finally {
      this.loading = false;
    }
  }
}
