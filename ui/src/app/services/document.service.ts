import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiResponse, PagedResult } from '../models/api-models';
import { Document } from '../models/asset-management.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  constructor(private api: ApiService) { }

  getDocuments(pageNumber = 1, pageSize = 10, searchTerm?: string, sortOrder?: string): Observable<ApiResponse<PagedResult<Document>>> {
    let url = `api/documents?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    if (sortOrder) url += `&sortOrder=${encodeURIComponent(sortOrder)}`;
    return this.api.get<PagedResult<Document>>(url);
  }

  createDocument(document: Partial<Document>): Observable<ApiResponse<number>> {
    return this.api.post<number>('api/documents', document);
  }

  updateDocument(id: number, document: Partial<Document>): Observable<ApiResponse<boolean>> {
    return this.api.put<boolean>(`api/documents/${id}`, document);
  }

  deleteDocument(id: number): Observable<ApiResponse<boolean>> {
    return this.api.delete<boolean>(`api/documents/${id}`);
  }
}
