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
    TooltipModule
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './roles.component.html',
  styleUrl: './roles.component.scss'
})
export class RolesComponent implements OnInit {
  roles: Role[] = [];
  permissions: Permission[] = [];
  groupedPermissions: { [key: string]: Permission[] } = {};
  selectedRole: Role | null = null;
  selectedPermissionIds: number[] = [];
  roleForm: FormGroup;
  displayPermissionsDialog = false;
  displayRoleDialog = false;
  isLoading = false;
  isSaving = false;
  isPermissionsLoading = false;
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
        if (!acc[curr.group]) acc[curr.group] = [];
        acc[curr.group].push(curr);
        return acc;
      }, {} as { [key: string]: Permission[] });
    });
  }
  showAddRoleDialog() {
    this.roleForm.reset();
    this.displayRoleDialog = true;
  }
  saveRole() {
    if (this.roleForm.invalid) return;
    this.isSaving = true;
    const role = this.roleForm.value;
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
  managePermissions(role: Role) {
    this.selectedRole = role;
    this.isPermissionsLoading = true;
    this.identityService.getRolePermissions(role.id).subscribe({
      next: (res) => {
        this.selectedPermissionIds = res.permissionIds;
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
}
