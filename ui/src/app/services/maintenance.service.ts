import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse, PagedResult } from '../models/api-models';
import { Maintenance } from '../models/asset-management.model';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceService {
  constructor(private api: ApiService) { }

  getMaintenances(pageNumber = 1, pageSize = 10, searchTerm?: string, sortOrder?: string): Observable<ApiResponse<PagedResult<Maintenance>>> {
    let url = `api/maintenances?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    if (sortOrder) url += `&sortOrder=${encodeURIComponent(sortOrder)}`;
    return this.api.get<PagedResult<Maintenance>>(url);
  }

  createMaintenance(maintenance: Partial<Maintenance>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/maintenances', maintenance);
  }

  updateMaintenance(id: number, maintenance: Partial<Maintenance>): Observable<ApiResponse<boolean>> {
    return this.api.put<boolean>(`api/maintenances/${id}`, maintenance);
  }

  deleteMaintenance(id: number): Observable<ApiResponse<boolean>> {
    return this.api.delete<boolean>(`api/maintenances/${id}`);
  }
}
