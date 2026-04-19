import { Component, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { LoadingService } from './services/loading.service';
import { delay } from 'rxjs/operators';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ToastModule, ConfirmDialogModule, ProgressSpinnerModule],
  template: `
    <!-- Global Loading Spinner -->
    <div *ngIf="loadingService.loading$ | async" class="loading-overlay">
        <p-progressSpinner strokeWidth="4"></p-progressSpinner>
    </div>
    <!-- Global Toast -->
    <p-toast></p-toast>
    <!-- Global Confirmation Dialog -->
    <p-confirmDialog [style]="{width: '450px'}"></p-confirmDialog>
    <router-outlet></router-outlet>
  `,
  styles: [`
    :host {
        display: block;
        height: 100vh;
        width: 100vw;
        margin: 0;
        padding: 0;
        overflow: hidden;
    }
    .loading-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100vw;
        height: 100vh;
        background: rgba(255, 255, 255, 0.8);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 10000;
        pointer-events: none;
    }
  `]
})
export class AppComponent {
  constructor(public loadingService: LoadingService) {}
}
