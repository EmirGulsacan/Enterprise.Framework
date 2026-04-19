import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable, map, shareReplay } from 'rxjs';
export enum LocationType {
  Region = 1,
  City = 2,
  Branch = 3,
  SubUnit = 4
}
export interface LocationNode {
  id: number;
  name: string;
  code: string;
  type: LocationType;
  registryNumbers?: string;
  children: LocationNode[];
}
@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private treeCache$?: Observable<LocationNode[]>;
  constructor(private api: ApiService) { }
  getOrganizationTree(forceRefresh = false): Observable<LocationNode[]> {
    if (forceRefresh || !this.treeCache$) {
      this.treeCache$ = this.api.get<LocationNode[]>('api/locations/tree').pipe(
        map(res => res.data),
        shareReplay(1)
      );
    }
    return this.treeCache$;
  }
  createLocation(location: any): Observable<number> {
    return this.api.post<number>('api/locations', location).pipe(
      map(res => {
        this.treeCache$ = undefined; 
        return res.data;
      })
    );
  }
}
