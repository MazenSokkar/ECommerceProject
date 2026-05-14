// features/shipping/services/shipping-api.service.ts

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Shipment, ShippingStatus } from '../models/Shipping.model';


@Injectable({ providedIn: 'root' })
export class ShippingApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Shipping`;
  private readonly adminUrl = `${environment.apiUrl}/admin/shipping`;

  // ── Customer ──────────────────────────────────────────────────
  getShipment(orderId: number): Observable<Shipment> {
    return this.http.get<Shipment>(`${this.baseUrl}/${orderId}`);
  }

  // ── Admin ─────────────────────────────────────────────────────
  createShipment(orderId: number): Observable<void> {
    return this.http.post<void>(`${this.adminUrl}/${orderId}`, {});
  }

  getShipmentAdmin(orderId: number): Observable<Shipment> {
    return this.http.get<Shipment>(`${this.adminUrl}/${orderId}`);
  }

  updateShipmentStatus(orderId: number, status: ShippingStatus): Observable<void> {
    return this.http.put<void>(`${this.adminUrl}/${orderId}/status?status=${status}`, {});
  }
}