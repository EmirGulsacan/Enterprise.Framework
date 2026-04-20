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

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, DialogModule, InputTextModule, ReactiveFormsModule],
  templateUrl: './employees.component.html'
})
export class EmployeesComponent implements OnInit {
  employees: Employee[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  displayDialog: boolean = false;
  
  employeeForm!: FormGroup;

  constructor(
    private employeeService: EmployeeService, 
    private notification: NotificationService,
    private fb: FormBuilder
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

  loadEmployees(event: TableLazyLoadEvent) {
    this.loading = true;
    const pageNumber = (event.first! / event.rows!) + 1;
    const pageSize = event.rows!;

    this.employeeService.getEmployees(pageNumber, pageSize).subscribe({
      next: (res) => {
        if (res.success) {
          this.employees = res.data.items;
          this.totalRecords = res.data.totalCount;
        }
        this.loading = false;
      },
      error: (err) => {
        this.notification.error('Çalışanlar yüklenemedi');
        this.loading = false;
      }
    });
  }

  showDialog() {
    this.employeeForm.reset();
    this.displayDialog = true;
  }

  saveEmployee() {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    const payload = this.employeeForm.value;

    this.employeeService.createEmployee(payload).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Çalışan başarıyla eklendi');
          this.displayDialog = false;
          this.loadEmployees({ first: 0, rows: 10 });
        }
      },
      error: (err) => {
        this.notification.error('Çalışan eklenirken hata oluştu');
      }
    });
  }
}
