import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/asset-management.model';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, DialogModule, InputTextModule, FormsModule],
  templateUrl: './employees.component.html'
})
export class EmployeesComponent implements OnInit {
  employees: Employee[] = [];
  displayDialog: boolean = false;
  newEmployee: Partial<Employee> = {};

  constructor(private employeeService: EmployeeService, private notification: NotificationService) {}

  ngOnInit() {
    this.loadEmployees();
  }

  loadEmployees() {
    this.employeeService.getEmployees().subscribe({
      next: (res) => {
        if (res.success) {
          this.employees = res.data.items;
        }
      },
      error: (err) => {
        this.notification.error('Çalışanlar yüklenemedi');
      }
    });
  }

  showDialog() {
    this.newEmployee = {};
    this.displayDialog = true;
  }

  saveEmployee() {
    this.employeeService.createEmployee(this.newEmployee).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Çalışan başarıyla eklendi');
          this.displayDialog = false;
          this.loadEmployees();
        }
      },
      error: (err) => {
        this.notification.error('Çalışan eklenirken hata oluştu');
      }
    });
  }
}
