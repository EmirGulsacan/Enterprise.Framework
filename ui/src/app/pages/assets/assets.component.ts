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
import { AuthService } from '../../services/auth.service';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { GenericGridComponent, GridColumn } from '../../shared/components/generic-grid/generic-grid.component';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, 
    ButtonModule, InputTextModule, DropdownModule, DialogModule, ConfirmDialogModule,
    GenericGridComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './assets.component.html'
})
export class AssetsComponent implements OnInit {
  assets: Asset[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  @ViewChild('grid') grid!: GenericGridComponent;

  assetForm!: FormGroup;
  displayDialog: boolean = false;
  editMode: boolean = false;
  selectedId: number | null = null;

  columns: GridColumn[] = [
    { field: 'name', header: 'İsim' },
    { field: 'serialNumber', header: 'Seri Numarası' },
    { field: 'purchaseDate', header: 'Satın Alma Tarihi', type: 'date' },
    { field: 'status', header: 'Durum' }
  ];

  statusOptions = [
    { label: 'Aktif', value: 'Active' },
    { label: 'Bakımda', value: 'Maintenance' },
    { label: 'Kullanım Dışı', value: 'Retired' }
  ];

  constructor(
    private assetService: AssetService, 
    private fb: FormBuilder,
    private notification: NotificationService,
    private confirmationService: ConfirmationService,
    public authService: AuthService
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

  showDialog() {
    this.editMode = false;
    this.selectedId = null;
    this.assetForm.reset({ status: 'Active' });
    this.displayDialog = true;
  }

  editAsset(asset: Asset) {
    this.editMode = true;
    this.selectedId = asset.id;
    this.assetForm.patchValue(asset);
    this.displayDialog = true;
  }

  deleteAsset(asset: Asset) {
    this.confirmationService.confirm({
      message: `${asset.name} isimli varlığı silmek istediğinize emin misiniz?`,
      header: 'Silme Onayı',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet, Sil',
      rejectLabel: 'Vazgeç',
      accept: () => {
        this.assetService.deleteAsset(asset.id).subscribe({
          next: (res) => {
            if (res.success) {
              this.notification.info('Varlık silindi');
              if (this.grid) this.grid.refresh();
            }
          },
          error: () => this.notification.error('Silme işlemi başarısız')
        });
      }
    });
  }

  exportExcel() {
    this.notification.info('Dışa aktarma işlemi başlatıldı...');
  }

  saveAsset() {
    if (this.assetForm.invalid) {
      this.assetForm.markAllAsTouched();
      return;
    }

    const payload = this.assetForm.value;

    if (this.editMode) {
      payload.id = this.selectedId; // FIX: ID Mismatch
      this.assetService.updateAsset(this.selectedId!, payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Varlık güncellendi');
            this.displayDialog = false;
            if (this.grid) this.grid.refresh();
          }
        },
        error: (err) => {
          this.notification.error('Kayıt sırasında hata oluştu');
        }
      });
    } else {
      this.assetService.createAsset(payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Varlık başarıyla eklendi');
            this.displayDialog = false;
            if (this.grid) this.grid.refresh();
          }
        },
        error: (err) => {
          this.notification.error('Kayıt sırasında hata oluştu');
        }
      });
    }
  }
}

