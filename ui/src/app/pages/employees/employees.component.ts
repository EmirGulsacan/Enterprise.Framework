import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Employee } from '../../models/asset-management.model';
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
  selector: 'app-employees',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, 
    ButtonModule, InputTextModule, DialogModule, ConfirmDialogModule,
    GenericGridComponent, GenericFormComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './employees.component.html'
})
export class EmployeesComponent implements OnInit {
  @ViewChild('grid') grid!: GenericGridComponent;
  employees: Employee[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  private employeeService: BaseCrudService<Employee>;

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

  selectedData: any = null;

  employeeFormFields: FormField[] = [
    { key: 'firstName', label: 'Ad', type: 'text', required: true },
    { key: 'lastName', label: 'Soyad', type: 'text', required: true },
    { key: 'email', label: 'E-Posta', type: 'email', required: true },
    { key: 'title', label: 'Unvan', type: 'text', required: true },
    { key: 'department', label: 'Departman', type: 'text', required: true }
  ];

  constructor(
    private apiService: ApiService,
    private notification: NotificationService,
    private confirmationService: ConfirmationService,
    public authService: AuthService
  ) {
    this.employeeService = new BaseCrudService<Employee>(this.apiService, '/api/v1/employees');
  }

  ngOnInit() {
  }

  loadEmployees(event: TableLazyLoadEvent) {
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

    let queryString = `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (filters['SortOrder']) queryString += `&SortOrder=${filters['SortOrder']}`;
    if (filters['SearchTerm']) queryString += `&SearchTerm=${encodeURIComponent(filters['SearchTerm'] as string)}`;
    if (filters['FiltersJson']) queryString += `&FiltersJson=${encodeURIComponent(filters['FiltersJson'] as string)}`;

    this.apiService.get<PagedResult<Employee>>(`/api/v1/employees${queryString}`, { headers: { 'X-Skip-Loading': 'true' } }).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.employees = res.data.items ?? [];
          this.totalRecords = res.data.totalCount ?? 0;
        } else {
          this.employees = [];
          this.totalRecords = 0;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.employees = [];
      }
    });
  }

  showDialog() {
    this.editMode = false;
    this.selectedData = null;
    this.displayDialog = true;
  }

  editEmployee(emp: Employee) {
    this.editMode = true;
    this.selectedData = { ...emp };
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
        this.employeeService.delete(emp.id).subscribe({
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

  saveEmployee(payload: any) {
    if (this.editMode) {
      payload.id = this.selectedData.id;
      this.employeeService.update(payload.id, payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Çalışan güncellendi');
            this.displayDialog = false;
            if (this.grid) this.grid.refresh();
          }
        },
        error: (err) => this.notification.error('Kayıt sırasında hata oluştu')
      });
    } else {
      this.employeeService.create(payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.notification.success('Çalışan başarıyla eklendi');
            this.displayDialog = false;
            if (this.grid) this.grid.refresh();
          }
        },
        error: (err) => this.notification.error('Kayıt sırasında hata oluştu')
      });
    }
  }
}

