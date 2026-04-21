import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { SkeletonModule } from 'primeng/skeleton';
import { ConfirmationService, MessageService } from 'primeng/api';
import { IdentityService, Role, Permission } from '../../../services/identity.service';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { OverlayPanelModule } from 'primeng/overlaypanel';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ReactiveFormsModule,
    TableModule, 
    ButtonModule, 
    DialogModule, 
    CheckboxModule, 
    ToastModule, 
    ConfirmDialogModule,
    InputTextModule,
    InputTextareaModule,
    SkeletonModule,
    TagModule,
    TooltipModule,
    OverlayPanelModule
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './roles.component.html',
  styleUrl: './roles.component.scss'
})
export class RolesComponent implements OnInit {
  roles: Role[] = [];
  permissions: Permission[] = [];
  groupedPermissions: { [key: string]: Permission[] } = {};
  filteredGroupedPermissions: { [key: string]: Permission[] } = {};
  permissionSearchText: string = '';
  
  permissionTranslations: Record<string, string> = {
    "Identity.Users.View": "Kullanıcı Görüntüleme",
    "Identity.Users.Write": "Kullanıcı Yönetimi",
    "Identity.Roles.View": "Rol Görüntüleme",
    "Identity.Roles.Write": "Rol Yönetimi",
    "Employees.View": "Çalışan Görüntüleme",
    "Employees.Write": "Çalışan Yönetimi",
    "Assets.View": "Varlık Görüntüleme",
    "Assets.Write": "Varlık Yönetimi",
    "Maintenances.View": "Bakım Görüntüleme",
    "Maintenances.Write": "Bakım Yönetimi",
    "Labors.View": "İşçilik Görüntüleme",
    "Labors.Write": "İşçilik Yönetimi",
    "Documents.View": "Doküman Görüntüleme",
    "Documents.Write": "Doküman Yönetimi",
    "Settings.System.View": "Sistem Görüntüleme",
    "Settings.System.Write": "Sistem Yönetimi"
  };
  selectedRole: Role | null = null;
  selectedPermissionIds: number[] = [];
  roleForm: FormGroup;
  displayPermissionsDialog = false;
  displayRoleDialog = false;
  isEditMode = false;
  isLoading = false;
  isSaving = false;
  isPermissionsLoading = false;
  selectedOverlayRole: Role | null = null;
  overlayGroupedPermissions: { [key: string]: Permission[] } = {};
  isOverlayLoading = false;

  constructor(
    private fb: FormBuilder,
    private identityService: IdentityService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {
    this.roleForm = this.fb.group({
      id: [null],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      description: ['', [Validators.maxLength(250)]]
    });
  }
  ngOnInit() {
    this.loadRoles();
    this.loadPermissions();
  }
  loadRoles() {
    this.isLoading = true;
    this.identityService.getRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }
  loadPermissions() {
    this.identityService.getPermissions().subscribe(perms => {
      this.permissions = perms;
      this.groupedPermissions = perms.reduce((acc, curr) => {
        if (!acc[curr.moduleName]) acc[curr.moduleName] = [];
        acc[curr.moduleName].push(curr);
        return acc;
      }, {} as { [key: string]: Permission[] });
      this.filteredGroupedPermissions = { ...this.groupedPermissions };
    });
  }
  showAddRoleDialog() {
    this.roleForm.reset();
    this.isEditMode = false;
    this.displayRoleDialog = true;
  }
  
  editRole(role: Role) {
    this.roleForm.patchValue(role);
    this.isEditMode = true;
    this.displayRoleDialog = true;
  }
  
  saveRole() {
    if (this.roleForm.invalid) return;
    this.isSaving = true;
    const role = this.roleForm.value;
    
    if (this.isEditMode && role.id) {
        this.identityService.updateRole(role).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Rol güncellendi' });
            this.displayRoleDialog = false;
            this.isSaving = false;
            this.loadRoles(); 
          },
          error: () => this.isSaving = false
        });
    } else {
        this.identityService.createRole(role).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Yeni rol oluşturuldu' });
            this.displayRoleDialog = false;
            this.isSaving = false;
            this.loadRoles(); 
          },
          error: () => this.isSaving = false
        });
    }
  }
  managePermissions(role: Role) {
    this.selectedRole = role;
    this.isPermissionsLoading = true;
    this.identityService.getRolePermissions(role.id).subscribe({
      next: (res) => {
        this.selectedPermissionIds = res.permissionIds;
        this.permissionSearchText = '';
        this.filterPermissions();
        this.displayPermissionsDialog = true;
        this.isPermissionsLoading = false;
      },
      error: () => this.isPermissionsLoading = false
    });
  }
  savePermissions() {
    if (!this.selectedRole) return;
    this.confirmationService.confirm({
      message: `${this.selectedRole.name} rolünün yetkilerini güncellemek istediğinize emin misiniz? Bu işlem bağlı tüm kullanıcıları etkileyecektir.`,
      header: 'Değişiklik Onayı',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet, Güncelle',
      rejectLabel: 'Vazgeç',
      accept: () => {
        this.isSaving = true;
        this.identityService.updateRolePermissions(this.selectedRole!.id, this.selectedPermissionIds).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Yetkiler güncellendi' });
            this.displayPermissionsDialog = false;
            this.isSaving = false;
            this.loadRoles();
          },
          error: () => {
            this.isSaving = false;
          }
        });
      }
    });
  }
  isPermissionSelected(id: number): boolean {
    return this.selectedPermissionIds.includes(id);
  }
  togglePermission(id: number) {
    const index = this.selectedPermissionIds.indexOf(id);
    if (index > -1) {
      this.selectedPermissionIds.splice(index, 1);
    } else {
      this.selectedPermissionIds.push(id);
    }
  }

  selectAllPermissions(event: any) {
    if (event.checked) {
      this.selectedPermissionIds = this.permissions.map(p => p.id);
    } else {
      this.selectedPermissionIds = [];
    }
  }

  isAllSelected(): boolean {
    if (this.permissions.length === 0) return false;
    return this.selectedPermissionIds.length === this.permissions.length;
  }

  selectModulePermissions(module: string, event: any) {
    const modulePermIds = this.groupedPermissions[module].map(p => p.id);
    if (event.checked) {
      const newIds = modulePermIds.filter(id => !this.selectedPermissionIds.includes(id));
      this.selectedPermissionIds = [...this.selectedPermissionIds, ...newIds];
    } else {
      this.selectedPermissionIds = this.selectedPermissionIds.filter(id => !modulePermIds.includes(id));
    }
  }

  isModuleAllSelected(module: string): boolean {
    const modulePermIds = this.groupedPermissions[module]?.map(p => p.id) || [];
    if (modulePermIds.length === 0) return false;
    return modulePermIds.every(id => this.selectedPermissionIds.includes(id));
  }
  
  filterPermissions() {
    if (!this.permissionSearchText) {
      this.filteredGroupedPermissions = { ...this.groupedPermissions };
      return;
    }
    const search = this.permissionSearchText.toLowerCase();
    this.filteredGroupedPermissions = {};
    for (const [module, perms] of Object.entries(this.groupedPermissions)) {
      const filtered = perms.filter(p => 
        (this.permissionTranslations[p.code] || p.name).toLowerCase().includes(search) || 
        p.name.toLowerCase().includes(search)
      );
      if (filtered.length > 0) {
        this.filteredGroupedPermissions[module] = filtered;
      }
    }
  }

  getPermissionName(id: number): string {
    const perm = this.permissions.find(p => p.id === id);
    if (!perm) return '';
    return this.permissionTranslations[perm.code] || perm.name;
  }
  
  getPermissionNameByCode(code: string, fallback: string): string {
    return this.permissionTranslations[code] || fallback;
  }

  loadOverlayPermissions(role: Role) {
    this.selectedOverlayRole = role;
    this.overlayGroupedPermissions = {};
    if (!role.permissionIds || role.permissionIds.length === 0) return;
    const rolePerms = this.permissions.filter(p => role.permissionIds!.includes(p.id));
    this.overlayGroupedPermissions = rolePerms.reduce((acc, curr) => {
      if (!acc[curr.moduleName]) acc[curr.moduleName] = [];
      acc[curr.moduleName].push(curr);
      return acc;
    }, {} as { [key: string]: Permission[] });
  }
}
