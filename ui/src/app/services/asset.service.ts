import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse } from '../models/api-models';
import { Asset } from '../models/asset-management.model';

@Injectable({
  providedIn: 'root'
})
export class AssetService {
  constructor(private api: ApiService) { }

  getAssets(pageNumber = 1, pageSize = 10): Observable<ApiResponse<any>> {
    return this.api.get<any>(`api/assets?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  createAsset(asset: Partial<Asset>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/assets', asset);
  }
}
