import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
export interface Branch {
  id: number;
  name: string;
  code: string;
  organizationId?: string;
}
@Injectable({
  providedIn: 'root'
})
export class BranchService {
  private apiUrl = `${environment.apiUrl}/api/branches`;
  constructor(private http: HttpClient) { }
  getBranches(page: number = 1, size: number = 10, queryParams: string = ''): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?PageNumber=${page}&PageSize=${size}${queryParams}`);
  }
  createBranch(branch: { name: string, code: string }): Observable<any> {
    return this.http.post<any>(this.apiUrl, branch);
  }
  updateBranch(id: number, branch: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, branch);
  }
  deleteBranch(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
