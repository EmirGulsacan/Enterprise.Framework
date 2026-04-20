import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AssetService } from '../../services/asset.service';
import { Asset, AssetStatus } from '../../models/asset-management.model';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, DialogModule, InputTextModule, DropdownModule, ReactiveFormsModule],
  templateUrl: './assets.component.html'
})
export class AssetsComponent implements OnInit {
  assets: Asset[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  displayDialog: boolean = false;
  
  assetForm!: FormGroup;

  statusOptions = [
    { label: 'Aktif', value: AssetStatus.Active },
    { label: 'Bakımda', value: AssetStatus.UnderMaintenance },
    { label: 'Pasif', value: AssetStatus.Inactive },
    { label: 'Kullanım Dışı', value: AssetStatus.Disposed }
  ];

  constructor(
    private assetService: AssetService, 
    private notification: NotificationService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    this.assetForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      serialNumber: ['', [Validators.required, Validators.maxLength(100)]],
      status: [AssetStatus.Active, Validators.required]
    });
  }

  loadAssets(event: TableLazyLoadEvent) {
    this.loading = true;
    const pageNumber = event.first! / event.rows! + 1;
    const pageSize = event.rows!;

    this.assetService.getAssets(pageNumber, pageSize).subscribe({
      next: (res) => {
        if (res.success) {
          this.assets = res.data.items;
          this.totalRecords = res.data.totalCount;
        }
        this.loading = false;
      },
      error: (err) => {
        this.notification.error('Varlıklar yüklenemedi');
        this.loading = false;
      }
    });
  }

  showDialog() {
    this.assetForm.reset({ status: AssetStatus.Active });
    this.displayDialog = true;
  }

  saveAsset() {
    if (this.assetForm.invalid) {
      this.assetForm.markAllAsTouched();
      return;
    }

    const payload = this.assetForm.value;

    this.assetService.createAsset(payload).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Varlık başarıyla eklendi');
          this.displayDialog = false;
          // Tabloyu sıfırlamak için dummy event gönderilebilir veya sayfa yenilenebilir
          this.loadAssets({ first: 0, rows: 10 });
        }
      },
      error: (err) => {
        this.notification.error('Varlık eklenirken hata oluştu');
      }
    });
  }
}
