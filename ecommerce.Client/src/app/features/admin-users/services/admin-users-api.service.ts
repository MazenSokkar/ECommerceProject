import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import {
  AdminUser,
  AdminUsersByRoleResponse,
  CreateAdminUserRequest,
  UpdateAdminUserRequest,
} from '../models/admin-users.model';

@Injectable({ providedIn: 'root' })
export class AdminUsersApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/admin/users`;

  getAll(includeDeleted: boolean): Observable<ApiResponse<AdminUsersByRoleResponse>> {
    return this.http.get<ApiResponse<AdminUsersByRoleResponse>>(
      `${this.base}?includeDeleted=${includeDeleted}`,
    );
  }

  create(request: CreateAdminUserRequest): Observable<ApiResponse<AdminUser>> {
    return this.http.post<ApiResponse<AdminUser>>(this.base, request);
  }

  update(id: number, request: UpdateAdminUserRequest): Observable<ApiResponse<AdminUser>> {
    return this.http.put<ApiResponse<AdminUser>>(`${this.base}/${id}`, request);
  }

  toggleStatus(id: number): Observable<ApiResponse<AdminUser>> {
    return this.http.put<ApiResponse<AdminUser>>(`${this.base}/${id}/status`, {});
  }

  delete(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/${id}`);
  }
}
