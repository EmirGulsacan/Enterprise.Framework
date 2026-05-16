import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService, User } from '../../services/user.service';
import { IdentityService, Role } from '../../services/identity.service';
import { NotificationService } from '../../services/notification.service';
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
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { AuthService } from '../../services/auth.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { GenericGridComponent, GridColumn, GridAction } from '../../shared/components/generic-grid/generic-grid.component';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, 
    ButtonModule, InputTextModule, TagModule, 
    TooltipModule, DialogModule, MultiSelectModule, 
    DropdownModule, InputSwitchModule, PasswordModule, SkeletonModule, OverlayPanelModule,
    GenericGridComponent
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
            <app-generic-grid 
                #grid
                title="Kullanıcı Yönetimi" 
                apiEndpoint="api/identity/users" 
                permissionModule="Identity.Users"
                [columns]="columns"
                [customActions]="customActions"
                (onAdd)="showCreateDialog()"
                (onEdit)="showEditDialog($event)"
                (onDelete)="deleteUser($event)"
                (onCustomAction)="handleCustomAction($event)">
            </app-generic-grid>
        </div>
        <!-- User Form Dialog -->
        <p-dialog [header]="editMode ? 'Kullanıcı Düzenle' : 'Yeni Kullanıcı'" [(visible)]="displayForm" [modal]="true" [style]="{width: '50vw'}" [breakpoints]="{'960px': '75vw', '640px': '100vw'}" class="p-fluid">
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
        <p-dialog header="Rol Atama" [(visible)]="displayRoleDialog" [modal]="true" [style]="{width: '40vw'}" [breakpoints]="{'960px': '60vw', '640px': '100vw'}" class="p-fluid">
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
                    appendTo="body"
                    class="w-full">
                </p-multiSelect>
            </div>
            <div class="flex justify-content-end gap-2 mt-4">
                <button pButton label="İptal" icon="pi pi-times" class="p-button-text" (click)="displayRoleDialog = false"></button>
                <button pButton label="Rolleri Güncelle" icon="pi pi-save" (click)="saveRoles()" [disabled]="isSaving" [loading]="isSaving"></button>
            </div>
        </p-dialog>

        <!-- Readonly Roles Overlay -->
        <p-overlayPanel #roleOp [showCloseIcon]="true" [style]="{width: '300px', maxHeight: '400px', overflowY: 'auto'}" styleClass="shadow-4 border-round-xl">
            <ng-template pTemplate="content">
                <div class="text-lg font-bold text-900 mb-3 border-bottom-1 border-200 pb-2">
                    {{selectedOverlayUser?.firstName}} Kullanıcısı Rolleri
                </div>
                <div class="flex flex-wrap gap-2">
                    <p-tag *ngFor="let roleId of selectedOverlayUser?.roleIds" 
                           [value]="getRoleName(roleId)" 
                           severity="info" 
                           [rounded]="true" 
                           styleClass="bg-blue-50 text-blue-700 border-1 border-blue-100"></p-tag>
                </div>
            </ng-template>
        </p-overlayPanel>
    </div>
  `
})
export class UserManagementComponent implements OnInit {
  @ViewChild('grid') grid!: GenericGridComponent;
  
  allRoles: Role[] = [];
  totalRecords: number = 0; // for stats
  isSaving: boolean = false;
  displayForm: boolean = false;
  displayRoleDialog: boolean = false;
  editMode: boolean = false;
  selectedUser: User | null = null;
  selectedRoleIds: number[] = [];
  userForm: FormGroup;

  columns: GridColumn[] = [
    { field: 'email', header: 'E-Posta' },
    { field: 'firstName', header: 'İsim' },
    { field: 'lastName', header: 'Soyisim' },
    { field: 'roles', header: 'Roller', type: 'tags' },
    { field: 'isActive', header: 'Durum', type: 'boolean' }
  ];

  customActions: GridAction[] = [
    { icon: 'pi pi-key', action: 'manage_roles', severity: 'warning', tooltip: 'Rolleri Yönet' }
  ];

  constructor(
    private userService: UserService,
    private identityService: IdentityService,
    private fb: FormBuilder,
    private confirmationService: ConfirmationService,
    private notification: NotificationService,
    public authService: AuthService
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
    this.getTotalUsersCount();
  }
  loadRoles() {
    this.identityService.getRoles().subscribe(roles => this.allRoles = roles);
  }
  getTotalUsersCount() {
    this.userService.getUsers(1, 1, '').subscribe(res => {
      if (res && res.data) {
        this.totalRecords = res.data.totalCount;
      }
    });
  }
  handleCustomAction(event: {action: string, row: User}) {
    if (event.action === 'manage_roles') {
      this.showRoleDialog(event.row);
    }
  }

  showCreateDialog() {
    this.editMode = false;
    this.userForm.reset({ isActive: true });
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.displayForm = true;
  }
  showEditDialog(user: User) {
    if (user.isSystemAdmin) {
      this.notification.error('Sistem yöneticisi düzenlenemez.');
      return;
    }
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
    if (this.editMode) {
      this.userService.updateUser(userData.id, userData).subscribe({
          next: () => {
              this.notification.success('Kullanıcı güncellendi.');
              this.displayForm = false;
              if (this.grid) this.grid.refresh();
              this.isSaving = false;
          },
          error: (err) => {
              this.notification.error(err?.error?.message || 'İşlem başarısız.');
              this.isSaving = false;
          }
      });
    } else {
      this.userService.createUser(userData).subscribe({
          next: () => {
              this.notification.success('Kullanıcı oluşturuldu.');
              this.displayForm = false;
              if (this.grid) this.grid.refresh();
              this.isSaving = false;
          },
          error: (err) => {
              this.notification.error(err?.error?.message || 'İşlem başarısız.');
              this.isSaving = false;
          }
      });
    }
  }
  showRoleDialog(user: User) {
    if (user.isSystemAdmin) {
      this.notification.error('Sistem yöneticisinin yetkileri değiştirilemez.');
      return;
    }
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
            if (this.grid) this.grid.refresh();
            this.isSaving = false;
        },
        error: (err) => {
            this.notification.error(err?.error?.message || 'Rol güncelleme başarısız.');
            this.isSaving = false;
        }
    });
  }
  deleteUser(user: User) {
    if (user && user.isSystemAdmin) {
      this.notification.error('Sistem yöneticisi silinemez.');
      return;
    }

    this.confirmationService.confirm({
        message: 'Bu kullanıcıyı silmek istediğinize emin misiniz?',
        header: 'Kullanıcı Sil',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
            this.userService.deleteUser(user.id).subscribe({
                next: () => {
                    this.notification.info('Kullanıcı silindi.');
                    if (this.grid) this.grid.refresh();
                },
                error: (err) => this.notification.error(err?.error?.message || 'Silme başarısız.')
            });
        }
    });
  }
}

