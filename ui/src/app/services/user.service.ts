import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-models';

export interface User {
  id: number;
  identityId: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
  roleIds: number[];
  roles?: string[];
  isSystemAdmin?: boolean;
}

export interface CreateUserPayload {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  roleIds: number[];
}

export interface UpdateUserPayload {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
  roleIds: number[];
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/api/identity/users`;
  constructor(private http: HttpClient) { }
  getUsers(page: number = 1, size: number = 10, queryParams: string = ''): Observable<ApiResponse<PagedResult<User>>> {
    return this.http.get<ApiResponse<PagedResult<User>>>(`${this.apiUrl}?PageNumber=${page}&PageSize=${size}${queryParams}`);
  }
  createUser(user: CreateUserPayload): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.apiUrl, user);
  }
  updateUser(id: number, user: UpdateUserPayload): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/${id}`, user);
  }
  deleteUser(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
  updateUserRoles(userId: number, roleIds: number[]): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/${userId}/roles`, roleIds);
  }
}
