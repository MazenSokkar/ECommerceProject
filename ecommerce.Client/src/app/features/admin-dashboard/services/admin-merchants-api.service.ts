import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Merchant, MerchantStatus } from '../models/merchant.model';

@Injectable({ providedIn: 'root' })
export class AdminMerchantsApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/Merchants`;

  getAll(): Observable<ApiResponse<Merchant[]>> {
    return this.http.get<ApiResponse<Merchant[]>>(this.base);
  }

  updateStatus(id: number, status: MerchantStatus): Observable<ApiResponse<Merchant>> {
    return this.http.put<ApiResponse<Merchant>>(`${this.base}/${id}/status`, { status });
  }
}
