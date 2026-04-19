import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService, User } from '../../services/user.service';
import { IdentityService, Role } from '../../services/identity.service';
import { NotificationService } from '../../services/notification.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ConfirmationService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { DialogModule } from 'primeng/dialog';
import { MultiSelectModule } from 'primeng/multiselect';
import { DropdownModule } from 'primeng/dropdown';
import { InputSwitchModule } from 'primeng/inputswitch';
import { PasswordModule } from 'primeng/password';
import { SkeletonModule } from 'primeng/skeleton';
@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, 
    TableModule, ButtonModule, InputTextModule, TagModule, 
    TooltipModule, DialogModule, MultiSelectModule, 
    DropdownModule, InputSwitchModule, PasswordModule, SkeletonModule
  ],
  template: `
    <div class="fadein animation-duration-500">
        <!-- Stats Row -->
        <div class="grid mb-4">
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-emerald-50 text-emerald-600">
                        <i class="pi pi-users"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Toplam Kullanıcı</div>
                        <div class="text-2xl font-bold">{{ totalRecords }}</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-blue-50 text-blue-600">
                        <i class="pi pi-user-check"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Aktif Oturum</div>
                        <div class="text-2xl font-bold">12</div>
                    </div>
                </div>
            </div>
            <div class="col-12 md:col-6 lg:col-4">
                <div class="premium-card stat-card">
                    <div class="stat-icon bg-orange-50 text-orange-600">
                        <i class="pi pi-shield"></i>
                    </div>
                    <div>
                        <div class="text-muted text-sm font-medium">Sistem Yöneticisi</div>
                        <div class="text-2xl font-bold">2</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="premium-card p-4">
            <div class="flex flex-column md:flex-row md:align-items-center justify-content-between mb-4 gap-3">
                <div>
                    <h1 class="text-2xl font-bold text-900 m-0">Kullanıcı Yönetimi</h1>
                    <p class="text-muted m-0">Sistemdeki tüm kullanıcıları ve yetkilerini yönetin.</p>
                </div>
                <div class="flex gap-2">
                    <span class="p-input-icon-left">
                        <i class="pi pi-search"></i>
                        <input type="text" pInputText placeholder="Kullanıcılarda ara..." (input)="onSearch($event)" class="p-inputtext-sm border-round-lg w-15rem" />
                    </span>
                    <button pButton label="Excel" icon="pi pi-file-excel" class="p-button-outlined p-button-secondary p-button-sm" (click)="exportExcel()"></button>
                    <button pButton label="Yeni Kullanıcı" icon="pi pi-user-plus" class="p-button-sm border-round-lg" (click)="showCreateDialog()"></button>
                </div>
            </div>
            <p-table 
                [value]="users" 
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
                        <th pSortableColumn="email">
                            KULLANICI <p-sortIcon field="email"></p-sortIcon>
                        </th>
                        <th>AD SOYAD</th>
                        <th>ROLLER</th>
                        <th style="width: 100px" class="text-center">DURUM</th>
                        <th style="width: 120px" class="text-center">EYLEM</th>
                    </tr>
                </ng-template>
                <ng-template pTemplate="body" let-user>
                    <tr>
                        <td class="font-bold text-slate-500">#{{ user.id }}</td>
                        <td>
                            <div class="flex flex-column">
                                <span class="font-bold text-900">{{ user.email }}</span>
                                <span class="text-xs text-muted">{{ user.identityId }}</span>
                            </div>
                        </td>
                        <td class="font-medium">{{ user.firstName }} {{ user.lastName }}</td>
                        <td>
                            <div class="flex flex-wrap gap-1">
                                <p-tag *ngFor="let roleId of user.roleIds" [value]="getRoleName(roleId)" severity="info" [rounded]="true"></p-tag>
                                <span *ngIf="!user.roleIds?.length" class="text-muted text-xs italic">Rol atanmamış</span>
                            </div>
                        </td>
                        <td class="text-center">
                            <p-tag [severity]="user.isActive ? 'success' : 'danger'" [value]="user.isActive ? 'AKTİF' : 'PASİF'" [rounded]="true"></p-tag>
                        </td>
                        <td class="text-center">
                            <div class="flex justify-content-center gap-1">
                                <button pButton icon="pi pi-pencil" 
                                        class="p-button-rounded p-button-info p-button-text p-button-sm" 
                                        (click)="showEditDialog(user)" 
                                        pTooltip="Düzenle">
                                </button>
                                <button pButton icon="pi pi-key" 
                                        class="p-button-rounded p-button-warning p-button-text p-button-sm" 
                                        (click)="showRoleDialog(user)" 
                                        pTooltip="Rolleri Yönet">
                                </button>
                                <button pButton icon="pi pi-trash" 
                                        class="p-button-rounded p-button-danger p-button-text p-button-sm" 
                                        (click)="deleteUser(user.id)" 
                                        pTooltip="Sil">
                                </button>
                            </div>
                        </td>
                    </tr>
                </ng-template>
                <ng-template pTemplate="emptymessage">
                    <tr>
                        <td colspan="6" class="text-center p-5 text-muted">
                            <i class="pi pi-users text-4xl mb-3 block"></i>
                            Kullanıcı bulunamadı.
                        </td>
                    </tr>
                </ng-template>
            </p-table>
        </div>
        <!-- User Form Dialog -->
        <p-dialog [header]="editMode ? 'Kullanıcı Düzenle' : 'Yeni Kullanıcı'" [(visible)]="displayForm" [modal]="true" [style]="{width: '450px'}" class="p-fluid">
            <form [formGroup]="userForm" (ngSubmit)="saveUser()">
                <div class="field mb-3">
                    <label for="username" class="font-bold block mb-1">Kullanıcı Adı</label>
                    <input type="text" pInputText id="username" formControlName="username" [readonly]="editMode" />
                </div>
                <div class="field mb-3">
                    <label for="email" class="font-bold block mb-1">E-Posta</label>
                    <input type="email" pInputText id="email" formControlName="email" />
                </div>
                <div class="formgrid grid">
                    <div class="field col mb-3">
                        <label for="firstName" class="font-bold block mb-1">Ad</label>
                        <input type="text" pInputText id="firstName" formControlName="firstName" />
                    </div>
                    <div class="field col mb-3">
                        <label for="lastName" class="font-bold block mb-1">Soyad</label>
                        <input type="text" pInputText id="lastName" formControlName="lastName" />
                    </div>
                </div>
                <div class="field mb-3" *ngIf="!editMode">
                    <label for="password" class="font-bold block mb-1">Şifre</label>
                    <p-password id="password" formControlName="password" [toggleMask]="true" [feedback]="false"></p-password>
                </div>
                <div class="field flex align-items-center gap-2 mb-3">
                    <p-inputSwitch formControlName="isActive"></p-inputSwitch>
                    <label class="font-medium">Hesap Aktif</label>
                </div>
                <div class="flex justify-content-end gap-2 mt-4">
                    <button pButton label="İptal" icon="pi pi-times" class="p-button-text" type="button" (click)="displayForm = false"></button>
                    <button pButton [label]="editMode ? 'Güncelle' : 'Kaydet'" icon="pi pi-check" type="submit" [disabled]="userForm.invalid || isSaving" [loading]="isSaving"></button>
                </div>
            </form>
        </p-dialog>
        <!-- Role Assignment Dialog -->
        <p-dialog header="Rol Atama" [(visible)]="displayRoleDialog" [modal]="true" [style]="{width: '400px'}" class="p-fluid">
            <div *ngIf="selectedUser" class="mb-4">
                <div class="text-900 font-bold mb-1">{{ selectedUser.firstName }} {{ selectedUser.lastName }}</div>
                <div class="text-sm text-muted">{{ selectedUser.email }}</div>
            </div>
            <div class="field">
                <label class="font-bold block mb-2">Atanacak Roller</label>
                <p-multiSelect 
                    [options]="allRoles" 
                    [(ngModel)]="selectedRoleIds" 
                    optionLabel="name" 
                    optionValue="id" 
                    placeholder="Rol seçiniz..."
                    display="chip"
                    class="w-full">
                </p-multiSelect>
            </div>
            <div class="flex justify-content-end gap-2 mt-4">
                <button pButton label="İptal" icon="pi pi-times" class="p-button-text" (click)="displayRoleDialog = false"></button>
                <button pButton label="Rolleri Güncelle" icon="pi pi-save" (click)="saveRoles()" [disabled]="isSaving" [loading]="isSaving"></button>
            </div>
        </p-dialog>
    </div>
  `
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  allRoles: Role[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  isSaving: boolean = false;
  rows: number = 10;
  searchTerm: string = '';
  currentSortField: string = 'id';
  currentSortOrder: string = 'desc';
  displayForm: boolean = false;
  displayRoleDialog: boolean = false;
  editMode: boolean = false;
  selectedUser: User | null = null;
  selectedRoleIds: number[] = [];
  userForm: FormGroup;
  constructor(
    private userService: UserService,
    private identityService: IdentityService,
    private fb: FormBuilder,
    private http: HttpClient,
    private confirmationService: ConfirmationService,
    private notification: NotificationService
  ) {
    this.userForm = this.fb.group({
      id: [null],
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      password: [''],
      isActive: [true]
    });
  }
  ngOnInit() {
    this.loadRoles();
  }
  loadRoles() {
    this.identityService.getRoles().subscribe(roles => this.allRoles = roles);
  }
  getRoleName(id: number): string {
    return this.allRoles.find(r => r.id === id)?.name || `ID: ${id}`;
  }
  onLazyLoad(event: any) {
    this.loading = true;
    this.rows = event.rows;
    if (event.sortField) {
        this.currentSortField = event.sortField;
        this.currentSortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
    }
    const page = (event.first / event.rows) + 1;
    this.loadUsers(page, event.rows);
  }
  onSearch(event: any) {
    this.searchTerm = event.target.value;
    this.loadUsers(1, this.rows);
  }
  loadUsers(page: number = 1, size: number = 10) {
    const sortQuery = this.currentSortField ? `&SortOrder=${this.currentSortField}_${this.currentSortOrder}` : '';
    const searchStr = this.searchTerm ? `&SearchTerm=${this.searchTerm}` : '';
    this.userService.getUsers(page, size, searchStr + sortQuery).subscribe({
      next: (res) => {
        if (res && res.data) {
            this.users = res.data.items || [];
            this.totalRecords = res.data.totalCount || 0;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Kullanıcılar yüklenirken hata oluştu.');
      }
    });
  }
  showCreateDialog() {
    this.editMode = false;
    this.userForm.reset({ isActive: true });
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.displayForm = true;
  }
  showEditDialog(user: User) {
    this.editMode = true;
    this.userForm.patchValue(user);
    this.userForm.get('username')?.setValue(user.email); 
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.updateValueAndValidity();
    this.displayForm = true;
  }
  saveUser() {
    if (this.userForm.invalid) return;
    this.isSaving = true;
    const userData = this.userForm.value;
    const request = this.editMode 
        ? this.userService.updateUser(userData.id, userData)
        : this.userService.createUser(userData);
    request.subscribe({
        next: () => {
            this.notification.success(this.editMode ? 'Kullanıcı güncellendi.' : 'Kullanıcı oluşturuldu.');
            this.displayForm = false;
            this.loadUsers();
            this.isSaving = false;
        },
        error: (err) => {
            this.notification.error(err?.error?.message || 'İşlem başarısız.');
            this.isSaving = false;
        }
    });
  }
  showRoleDialog(user: User) {
    this.selectedUser = user;
    this.selectedRoleIds = [...(user.roleIds || [])];
    this.displayRoleDialog = true;
  }
  saveRoles() {
    if (!this.selectedUser) return;
    this.isSaving = true;
    this.userService.updateUserRoles(this.selectedUser.id, this.selectedRoleIds).subscribe({
        next: () => {
            this.notification.success('Roller başarıyla güncellendi.');
            this.displayRoleDialog = false;
            this.loadUsers();
            this.isSaving = false;
        },
        error: (err) => {
            this.notification.error(err?.error?.message || 'Rol güncelleme başarısız.');
            this.isSaving = false;
        }
    });
  }
  deleteUser(id: number) {
    this.confirmationService.confirm({
        message: 'Bu kullanıcıyı silmek istediğinize emin misiniz?',
        header: 'Kullanıcı Sil',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
            this.userService.deleteUser(id).subscribe({
                next: () => {
                    this.notification.info('Kullanıcı silindi.');
                    this.loadUsers();
                },
                error: (err) => this.notification.error(err?.error?.message || 'Silme başarısız.')
            });
        }
    });
  }
  exportExcel() {
    this.http.get(`${environment.apiUrl}/api/identity/users/export`, { responseType: 'blob' }).subscribe({
        next: (blob) => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'users.xlsx';
            a.click();
            window.URL.revokeObjectURL(url);
            this.notification.success('Excel indirildi.');
        },
        error: () => this.notification.error('Hata oluştu.')
    });
  }
}
