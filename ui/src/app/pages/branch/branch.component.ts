import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BranchService, Branch } from '../../services/branch.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ConfirmationService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';
@Component({
  selector: 'app-branch',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule, DialogModule, TooltipModule, TagModule],
  template: `
    <div class="fadein animation-duration-500">
        <!-- Stats Row -->
        <div class="grid mb-4">
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-indigo-50 text-indigo-600">
                        <i class="pi pi-building"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Toplam Şube</div>
                        <div class="text-2xl font-bold">{{ totalRecords }}</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-purple-50 text-purple-600">
                        <i class="pi pi-map-marker"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Bölge Sayısı</div>
                        <div class="text-2xl font-bold">12</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-teal-50 text-teal-600">
                        <i class="pi pi-chart-line"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Aktif Şube Oranı</div>
                        <div class="text-2xl font-bold">%100</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="premium-card p-4">
            <div class="flex flex-column md:flex-row md:align-items-center justify-content-between mb-4 gap-3">
                <div>
                    <h1 class="text-2xl font-bold text-900 m-0">Şube Yönetimi</h1>
                    <p class="text-muted m-0">Organizasyon yapınızdaki tüm şubeleri buradan izleyin.</p>
                </div>
                <div class="flex gap-2">
                    <span class="p-input-icon-left">
                        <i class="pi pi-search"></i>
                        <input type="text" pInputText placeholder="Şubelerde ara..." (input)="onSearch($event)" class="p-inputtext-sm border-round-lg w-15rem" />
                    </span>
                    <button pButton label="Excel" icon="pi pi-file-excel" class="p-button-outlined p-button-secondary p-button-sm" (click)="exportExcel()"></button>
                    <button pButton label="Yeni Şube" icon="pi pi-plus" class="p-button-accent p-button-sm" (click)="showDialog()"></button>
                </div>
            </div>
            <p-table 
                [value]="branches" 
                [lazy]="true" 
                (onLazyLoad)="onLazyLoad($event)"
                [paginator]="true" 
                [rows]="rows" 
                [totalRecords]="totalRecords" 
                [loading]="loading"
                [responsiveLayout]="'scroll'"
                styleClass="p-datatable-gridlines p-datatable-sm p-datatable-striped"
                [rowsPerPageOptions]="[5,10,20,50]"
                [showCurrentPageReport]="true"
                [alwaysShowPaginator]="true"
                currentPageReportTemplate="{totalRecords} kayıttan {first} - {last} arası gösteriliyor"
            >
                <ng-template pTemplate="header">
                    <tr>
                        <th style="width: 80px" pSortableColumn="id">ID <p-sortIcon field="id"></p-sortIcon></th>
                        <th pSortableColumn="name">
                            ŞUBE ADI <p-sortIcon field="name"></p-sortIcon>
                            <p-columnFilter type="text" field="name" display="menu" [showMatchModes]="false" [showOperator]="false" [showAddButton]="false"></p-columnFilter>
                        </th>
                        <th style="width: 150px" pSortableColumn="code">
                            ŞUBE KODU <p-sortIcon field="code"></p-sortIcon>
                            <p-columnFilter type="text" field="code" display="menu" [showMatchModes]="false" [showOperator]="false" [showAddButton]="false"></p-columnFilter>
                        </th>
                        <th style="width: 200px">ORGANİZASYON ID</th>
                        <th style="width: 100px" class="text-center">DURUM</th>
                        <th style="width: 120px" class="text-center">EYLEM</th>
                    </tr>
                </ng-template>
                <ng-template pTemplate="body" let-branch>
                    <tr>
                        <td class="font-bold text-slate-500">#{{ branch.id }}</td>
                        <td class="font-medium text-900">{{ branch.name }}</td>
                        <td><code class="bg-gray-100 px-2 py-1 border-round text-sm">{{ branch.code }}</code></td>
                        <td class="text-muted text-sm">{{ branch.organizationId || 'Genel' }}</td>
                        <td class="text-center">
                            <p-tag severity="success" value="AKTİF" [rounded]="true"></p-tag>
                        </td>
                        <td class="text-center">
                            <div class="flex justify-content-center gap-1">
                                <button pButton icon="pi pi-pencil" 
                                        class="p-button-rounded p-button-info p-button-text p-button-sm" 
                                        (click)="editBranch(branch)" 
                                        pTooltip="Düzenle">
                                </button>
                                <button pButton icon="pi pi-trash" 
                                        class="p-button-rounded p-button-danger p-button-text p-button-sm" 
                                        (click)="deleteBranch(branch.id)" 
                                        pTooltip="Sil">
                                </button>
                            </div>
                        </td>
                    </tr>
                </ng-template>
                <ng-template pTemplate="emptymessage">
                    <tr>
                        <td colspan="6" class="text-center p-5 text-muted">
                            <i class="pi pi-building text-4xl mb-3 block"></i>
                            Henüz bir şube tanımlanmamış.
                        </td>
                    </tr>
                </ng-template>
            </p-table>
        </div>
        <p-dialog [header]="editMode ? 'Şubeyi Düzenle' : 'Yeni Şube Ekle'" [(visible)]="displayDialog" [modal]="true" [style]="{width: '450px'}" styleClass="p-fluid" [draggable]="false" [resizable]="false">
            <div class="field mt-3">
                <label for="name" class="font-bold mb-2 block">Şube Adı</label>
                <input pInputText id="name" [(ngModel)]="branchModel.name" placeholder="Örn: Kadıköy Bölge Müd." autofocus />
            </div>
            <div class="field mt-3">
                <label for="code" class="font-bold mb-2 block">Şube Kodu</label>
                <input pInputText id="code" [(ngModel)]="branchModel.code" placeholder="Örn: BR-001" />
            </div>
            <ng-template pTemplate="footer">
                <button pButton label="İptal" icon="pi pi-times" (click)="displayDialog=false" class="p-button-text p-button-secondary"></button>
                <button pButton [label]="editMode ? 'Güncelle' : 'Kaydet'" icon="pi pi-check" (click)="saveBranch()" [disabled]="!branchModel.name || !branchModel.code" style="background: var(--accent-color); border: none"></button>
            </ng-template>
        </p-dialog>
    </div>
  `
})
export class BranchComponent implements OnInit {
  branches: Branch[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  rows: number = 10;
  displayDialog: boolean = false;
  editMode: boolean = false;
  branchModel: any = { id: 0, name: '', code: '' };
  searchTerm: string = '';
  currentSortField: string = 'id';
  currentSortOrder: string = 'desc';
  constructor(
    private branchService: BranchService,
    private http: HttpClient,
    private confirmationService: ConfirmationService,
    private notification: NotificationService,
    public authService: AuthService
  ) {}
  ngOnInit() {
  }
  onLazyLoad(event: any) {
    this.loading = true;
    this.rows = event.rows;
    if (event.sortField) {
        this.currentSortField = event.sortField;
        this.currentSortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
    }
    if (event.filters) {
        if (event.filters.name) this.searchTerm = event.filters.name.value || '';
        else if (event.filters.code) this.searchTerm = event.filters.code.value || '';
    }
    const page = (event.first / event.rows) + 1;
    this.loadBranches(page, event.rows);
  }
  onSearch(event: any) {
    this.searchTerm = event.target.value;
    this.loadBranches(1, this.rows);
  }
  loadBranches(page: number = 1, size: number = 10) {
    const sortQuery = this.currentSortField ? `&SortOrder=${this.currentSortField}_${this.currentSortOrder}` : '';
    const searchStr = this.searchTerm ? `&SearchTerm=${this.searchTerm}` : '';
    this.branchService.getBranches(page, size, searchStr + sortQuery).subscribe({
      next: (res) => {
        if (res && res.data) {
            this.branches = res.data.items || [];
            this.totalRecords = res.data.totalCount || 0;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Şubeler yüklenirken hata oluştu.');
      }
    });
  }
  showDialog() {
    this.editMode = false;
    this.branchModel = { id: 0, name: '', code: '' };
    this.displayDialog = true;
  }
  editBranch(branch: Branch) {
    this.editMode = true;
    this.branchModel = { ...branch };
    this.displayDialog = true;
  }
  saveBranch() {
    if (this.editMode) {
        this.branchService.updateBranch(this.branchModel.id, this.branchModel).subscribe({
            next: () => {
                this.notification.success('Şube güncellendi.');
                this.displayDialog = false;
                this.loadBranches();
            },
            error: () => this.notification.error('Güncelleme sırasında hata oluştu.')
        });
    } else {
        this.branchService.createBranch(this.branchModel).subscribe({
            next: () => {
                this.notification.success('Şube başarıyla eklendi.');
                this.displayDialog = false;
                this.loadBranches();
            },
            error: () => this.notification.error('Şube eklenirken hata oluştu.')
        });
    }
  }
  deleteBranch(id: number) {
    this.confirmationService.confirm({
        message: 'Bu şubeyi silmek istediğinize emin misiniz? Bu işlem geri alınamaz.',
        header: 'Şube Silme Onayı',
        icon: 'pi pi-exclamation-triangle',
        acceptLabel: 'Evet, Sil',
        rejectLabel: 'İptal',
        acceptButtonStyleClass: 'p-button-danger p-button-text',
        rejectButtonStyleClass: 'p-button-text p-button-secondary',
        accept: () => {
            this.branchService.deleteBranch(id).subscribe({
                next: () => {
                    this.notification.info('Şube başarıyla silindi.');
                    this.loadBranches();
                },
                error: () => this.notification.error('Silme işlemi başarısız.')
            });
        }
    });
  }
  exportExcel() {
    this.http.get(`${environment.apiUrl}/api/branches/export`, { responseType: 'blob' }).subscribe({
        next: (blob) => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'branches.xlsx';
            a.click();
            window.URL.revokeObjectURL(url);
            this.notification.success('Excel dosyası indirildi.');
        },
        error: () => this.notification.error('Excel indirilirken hata oluştu.')
    });
  }
}
