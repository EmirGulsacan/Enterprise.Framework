import { Observable } from 'rxjs';
import { ApiService } from '../../services/api.service';
import { ApiResponse, PagedResult } from '../../models/api-models';

export class BaseCrudService<TDto, TCreateDto = Partial<TDto>, TUpdateDto = Partial<TDto>> {
  constructor(protected api: ApiService, protected endpointPath: string) {
  }

  getAll(filters?: Record<string, unknown>): Observable<ApiResponse<PagedResult<TDto>>> {
    let queryParams = '';
    if (filters) {
      if (typeof filters === 'string') {
        queryParams = `?filtersJson=${filters}`;
      } else {
        queryParams = `?filtersJson=${JSON.stringify(filters)}`;
      }
    }
    return this.api.get<PagedResult<TDto>>(`${this.endpointPath}${queryParams}`);
  }

  getById(id: number): Observable<ApiResponse<TDto>> {
    return this.api.get<TDto>(`${this.endpointPath}/${id}`);
  }

  create(data: TCreateDto): Observable<ApiResponse<number>> {
    return this.api.post<number>(this.endpointPath, data);
  }

  update(id: number, data: TUpdateDto): Observable<ApiResponse<boolean>> {
    return this.api.put<boolean>(`${this.endpointPath}/${id}`, data);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.api.delete<boolean>(`${this.endpointPath}/${id}`);
  }
}
