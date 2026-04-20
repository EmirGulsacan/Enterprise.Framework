import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-models';
import { Employee } from '../models/asset-management.model';

import { PagedResult } from '../models/api-models';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  constructor(private api: ApiService) { }

  getEmployees(pageNumber = 1, pageSize = 10, searchTerm?: string, sortOrder?: string): Observable<ApiResponse<PagedResult<Employee>>> {
    let url = `api/employees?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    if (sortOrder) url += `&sortOrder=${encodeURIComponent(sortOrder)}`;
    return this.api.get<PagedResult<Employee>>(url);
  }

  createEmployee(employee: Partial<Employee>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/employees', employee);
  }

  updateEmployee(id: number, employee: Partial<Employee>): Observable<ApiResponse<boolean>> {
    return this.api.put<boolean>(`api/employees/${id}`, employee);
  }

  deleteEmployee(id: number): Observable<ApiResponse<boolean>> {
    return this.api.delete<boolean>(`api/employees/${id}`);
  }
}
