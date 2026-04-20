import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse, PagedResult } from '../models/api-models';
import { Labor } from '../models/asset-management.model';

@Injectable({
  providedIn: 'root'
})
export class LaborService {
  constructor(private api: ApiService) { }

  getLabors(pageNumber = 1, pageSize = 10, searchTerm?: string, sortOrder?: string): Observable<ApiResponse<PagedResult<Labor>>> {
    let url = `api/labors?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    if (sortOrder) url += `&sortOrder=${encodeURIComponent(sortOrder)}`;
    return this.api.get<PagedResult<Labor>>(url);
  }

  createLabor(labor: Partial<Labor>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/labors', labor);
  }

  updateLabor(id: number, labor: Partial<Labor>): Observable<ApiResponse<boolean>> {
    return this.api.put<boolean>(`api/labors/${id}`, labor);
  }

  deleteLabor(id: number): Observable<ApiResponse<boolean>> {
    return this.api.delete<boolean>(`api/labors/${id}`);
  }
}
