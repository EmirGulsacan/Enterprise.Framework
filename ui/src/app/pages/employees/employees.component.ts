import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/asset-management.model';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { GenericGridComponent, GridColumn } from '../../shared/components/generic-grid/generic-grid.component';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, 
    ButtonModule, InputTextModule, DialogModule, ConfirmDialogModule,
    GenericGridComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './employees.component.html'
})
export class EmployeesComponent implements OnInit {
  @ViewChild('grid') grid!: GenericGridComponent;

  employeeForm!: FormGroup;
  displayDialog: boolean = false;
  editMode: boolean = false;
  selectedId: number | null = null;

  columns: GridColumn[] = [
    { field: 'firstName', header: 'İsim' },
    { field: 'lastName', header: 'Soyisim' },
    { field: 'email', header: 'Email' },
    { field: 'title', header: 'Unvan' },
    { field: 'department', header: 'Departman' }
  ];

  constructor(
    private employeeService: EmployeeService, 
    private fb: FormBuilder,
    private notification: NotificationService,
    private confirmationService: ConfirmationService,
    public authService: AuthService
  ) {}

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    this.employeeForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
      title: ['', Validators.required],
      department: ['', Validators.required]
    });
  }

  showDialog() {
    this.editMode = false;
    this.selectedId = null;
    this.employeeForm.reset();
    this.displayDialog = true;
  }

  editEmployee(emp: Employee) {
    this.editMode = true;
    this.selectedId = emp.id;
    this.employeeForm.patchValue(emp);
    this.displayDialog = true;
  }

  deleteEmployee(emp: Employee) {
    this.confirmationService.confirm({
      message: `${emp.firstName} ${emp.lastName} isimli çalışanı silmek istediğinize emin misiniz?`,
      header: 'Silme Onayı',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet, Sil',
      rejectLabel: 'Vazgeç',
      accept: () => {
        this.employeeService.deleteEmployee(emp.id).subscribe({
          next: (res) => {
            if (res.success) {
              this.notification.info('Çalışan silindi');
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

  saveEmployee() {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    const payload = this.employeeForm.value;

    if (this.editMode) {
      payload.id = this.selectedId; // FIX: ID Mismatch
      this.employeeService.updateEmployee(this.selectedId!, payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Çalışan güncellendi');
            this.displayDialog = false;
            if (this.grid) this.grid.refresh();
          }
        },
        error: (err) => {
          this.notification.error('Kayıt sırasında hata oluştu');
        }
      });
    } else {
      this.employeeService.createEmployee(payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Çalışan başarıyla eklendi');
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

