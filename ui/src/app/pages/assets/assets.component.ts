import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Asset, AssetStatus } from '../../models/asset-management.model';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';
import { BaseCrudService } from '../../shared/services/base-crud.service';
import { PagedResult } from '../../models/api-models';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { GenericGridComponent, GridColumn } from '../../shared/components/generic-grid/generic-grid.component';
import { GenericFormComponent, FormField } from '../../shared/components/generic-form/generic-form.component';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, 
    ButtonModule, InputTextModule, DropdownModule, DialogModule, ConfirmDialogModule,
    GenericGridComponent, GenericFormComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './assets.component.html'
})
export class AssetsComponent implements OnInit {
  assets: Asset[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  @ViewChild('grid') grid!: GenericGridComponent;
  
  private assetService: BaseCrudService<Asset>;

  assetForm!: FormGroup;
  displayDialog: boolean = false;
  editMode: boolean = false;
  selectedData: any = null;

  columns: GridColumn[] = [
    { field: 'name', header: 'İsim' },
    { field: 'serialNumber', header: 'Seri Numarası' },
    { field: 'purchaseDate', header: 'Satın Alma Tarihi', type: 'date' },
    { field: 'status', header: 'Durum' }
  ];

  statusOptions = [
    { label: 'Aktif', value: 'Active' },
    { label: 'Bakımda', value: 'UnderMaintenance' },
    { label: 'Pasif', value: 'Inactive' },
    { label: 'Kullanım Dışı (Hurda)', value: 'Disposed' }
  ];

  assetFormFields: FormField[] = [
    { key: 'name', label: 'İsim', type: 'text', required: true },
    { key: 'serialNumber', label: 'Seri Numarası', type: 'text', required: true },
    { key: 'purchaseDate', label: 'Satın Alma Tarihi', type: 'date', required: true },
    { key: 'status', label: 'Durum', type: 'dropdown', options: this.statusOptions, optionLabel: 'label', optionValue: 'value', required: true }
  ];

  constructor(
    private apiService: ApiService,
    private notification: NotificationService,
    private confirmationService: ConfirmationService,
    public authService: AuthService
  ) {
    this.assetService = new BaseCrudService<Asset>(this.apiService, '/api/v1/assets');
  }

  ngOnInit() {
  }

  loadAssets(event: TableLazyLoadEvent) {
    this.loading = true;
    const pageNumber = event.first !== undefined && event.rows ? (event.first / event.rows) + 1 : 1;
    const pageSize = event.rows || 10;
    
    let filters: Record<string, unknown> = {
      pageNumber,
      pageSize
    };

    if (event.sortField) {
      const sortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
      filters['SortOrder'] = `${event.sortField}_${sortOrder}`;
    }

    if (event.globalFilter) {
      filters['SearchTerm'] = event.globalFilter;
    }

    // Process column filters
    if (event.filters && Object.keys(event.filters).length > 0) {
      const customFilters: Record<string, unknown> = {};
      Object.keys(event.filters).forEach(key => {
        const filterMeta = event.filters![key];
        let filterValue: unknown = null;
        if (Array.isArray(filterMeta) && filterMeta.length > 0) {
          filterValue = filterMeta[0].value;
        } else if (filterMeta && !Array.isArray(filterMeta)) {
          filterValue = filterMeta.value;
        }

        if (filterValue !== null && filterValue !== undefined && filterValue !== '') {
          customFilters[key] = filterValue;
        }
      });
      if (Object.keys(customFilters).length > 0) {
        filters['FiltersJson'] = JSON.stringify(customFilters);
      }
    }

    // Note: To pass parameters effectively to the BaseCrudService GET, we need to adapt it.
    // For now, let's build the query string manually since BaseCrudService expects a string or object.
    let queryString = `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (filters['SortOrder']) queryString += `&SortOrder=${filters['SortOrder']}`;
    if (filters['SearchTerm']) queryString += `&SearchTerm=${encodeURIComponent(filters['SearchTerm'] as string)}`;
    if (filters['FiltersJson']) queryString += `&FiltersJson=${encodeURIComponent(filters['FiltersJson'] as string)}`;

    // Using apiService directly is easier for complex query params than modifying BaseCrudService's getAll signature right now, 
    // or we can just use ApiService.
    this.apiService.get<PagedResult<Asset>>(`/api/v1/assets${queryString}`, { headers: { 'X-Skip-Loading': 'true' } }).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.assets = res.data.items ?? [];
          this.totalRecords = res.data.totalCount ?? 0;
        } else {
          this.assets = [];
          this.totalRecords = 0;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.assets = [];
      }
    });
  }

  showDialog() {
    this.editMode = false;
    this.selectedData = null;
    this.displayDialog = true;
  }

  editAsset(asset: Asset) {
    this.editMode = true;
    this.selectedData = { ...asset };
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
        this.assetService.delete(asset.id).subscribe({
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

  saveAsset(payload: any) {
    if (this.editMode) {
      payload.id = this.selectedData.id;
      this.assetService.update(payload.id, payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Varlık güncellendi');
            this.displayDialog = false;
            if (this.grid) this.grid.refresh();
          }
        },
        error: (err) => this.notification.error('Kayıt sırasında hata oluştu')
      });
    } else {
      this.assetService.create(payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Varlık başarıyla eklendi');
            this.displayDialog = false;
            if (this.grid) this.grid.refresh();
          }
        },
        error: (err) => this.notification.error('Kayıt sırasında hata oluştu')
      });
    }
  }
}

