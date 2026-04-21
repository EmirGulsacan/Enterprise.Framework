import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable, map, shareReplay, of } from 'rxjs';
export interface Role {
  id: number;
  name: string;
  description: string;
  permissionIds: number[];
}
export interface Permission {
  id: number;
  name: string;
  description: string;
  moduleName: string;
  code: string;
}
export interface RoleWithPermissions extends Role {
  permissionIds: number[];
}
@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  private rolesCache$?: Observable<Role[]>;
  private permissionsCache$?: Observable<Permission[]>;
  constructor(private api: ApiService) { }
  getRoles(forceRefresh = false): Observable<Role[]> {
    if (forceRefresh || !this.rolesCache$) {
      this.rolesCache$ = this.api.get<Role[]>('api/identity/roles').pipe(
        map(res => res.data),
        shareReplay(1)
      );
    }
    return this.rolesCache$;
  }
  createRole(role: Partial<Role>): Observable<number> {
    return this.api.post<number>('api/identity/roles', role).pipe(
      map(res => {
        this.rolesCache$ = undefined; 
        return res.data;
      })
    );
  }
  updateRole(role: Partial<Role>): Observable<boolean> {
    return this.api.put<boolean>(`api/identity/roles/${role.id}`, role).pipe(
      map(res => {
        this.rolesCache$ = undefined; 
        return res.data;
      })
    );
  }
  getPermissions(forceRefresh = false): Observable<Permission[]> {
    if (forceRefresh || !this.permissionsCache$) {
      this.permissionsCache$ = this.api.get<Permission[]>('api/identity/permissions').pipe(
        map(res => res.data),
        shareReplay(1)
      );
    }
    return this.permissionsCache$;
  }
  getRolePermissions(roleId: number): Observable<RoleWithPermissions> {
    return this.api.get<RoleWithPermissions>(`api/identity/roles/${roleId}/permissions`).pipe(map(res => res.data));
  }
  updateRolePermissions(roleId: number, permissionIds: number[]): Observable<boolean> {
    return this.api.put<boolean>(`api/identity/roles/${roleId}/permissions`, { roleId, permissionIds }).pipe(map(res => res.data));
  }
}
