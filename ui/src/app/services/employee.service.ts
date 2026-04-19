import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-models';
import { Employee } from '../models/asset-management.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  constructor(private api: ApiService) { }

  getEmployees(pageNumber = 1, pageSize = 10): Observable<ApiResponse<any>> {
    return this.api.get<any>(`api/employees?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  createEmployee(employee: Partial<Employee>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/employees', employee);
  }
}
