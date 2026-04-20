import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-models';
import { Asset } from '../models/asset-management.model';

import { PagedResult } from '../models/api-models';

@Injectable({
  providedIn: 'root'
})
export class AssetService {
  constructor(private api: ApiService) { }

  getAssets(pageNumber = 1, pageSize = 10, searchTerm?: string, sortOrder?: string): Observable<ApiResponse<PagedResult<Asset>>> {
    let url = `api/assets?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    if (sortOrder) url += `&sortOrder=${encodeURIComponent(sortOrder)}`;
    return this.api.get<PagedResult<Asset>>(url);
  }

  createAsset(asset: Partial<Asset>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/assets', asset);
  }

  updateAsset(id: number, asset: Partial<Asset>): Observable<ApiResponse<boolean>> {
    return this.api.put<boolean>(`api/assets/${id}`, asset);
  }

  deleteAsset(id: number): Observable<ApiResponse<boolean>> {
    return this.api.delete<boolean>(`api/assets/${id}`);
  }
}
