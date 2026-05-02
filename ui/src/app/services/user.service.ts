import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
export interface User {
  id: number;
  identityId: string;
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
  roleIds: number[];
  roles?: string[];
  isSystemAdmin?: boolean;
}
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/api/identity/users`;
  constructor(private http: HttpClient) { }
  getUsers(page: number = 1, size: number = 10, queryParams: string = ''): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?PageNumber=${page}&PageSize=${size}${queryParams}`);
  }
  createUser(user: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, user);
  }
  updateUser(id: number, user: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, user);
  }
  deleteUser(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
  updateUserRoles(userId: number, roleIds: number[]): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${userId}/roles`, roleIds);
  }
}
