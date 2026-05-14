import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';
import { Banner, CreateBannerRequest, UpdateBannerRequest } from '../../shared/models/banner.model';

@Injectable({ providedIn: 'root' })
export class BannersApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/banners`;

  getActive(): Observable<ApiResponse<Banner[]>> {
    return this.http.get<ApiResponse<Banner[]>>(this.baseUrl);
  }

  getAllAdmin(): Observable<ApiResponse<Banner[]>> {
    return this.http.get<ApiResponse<Banner[]>>(`${this.baseUrl}/admin`);
  }

  getById(id: number): Observable<ApiResponse<Banner>> {
    return this.http.get<ApiResponse<Banner>>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateBannerRequest): Observable<ApiResponse<Banner>> {
    return this.http.post<ApiResponse<Banner>>(this.baseUrl, request);
  }

  update(id: number, request: UpdateBannerRequest): Observable<ApiResponse<Banner>> {
    return this.http.put<ApiResponse<Banner>>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);
  }
}
