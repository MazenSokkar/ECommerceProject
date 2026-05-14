// features/orders/services/order-api.service.ts

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { OrderDetail, OrderSummary, PlaceOrderRequest, UpdateOrderStatusRequest } from '../models/Order.model';

@Injectable({ providedIn: 'root' })
export class OrderApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Order`;
  private readonly adminUrl = `${environment.apiUrl}/admin/orders`;

  // ── Customer ──────────────────────────────────────────────────
placeOrder(request: { shippingAddressId: number; notes: string }): Observable<any> {
  return this.http.post<any>(this.baseUrl, request);
}

getMyAddresses(userId: number): Observable<any> {
  return this.http.get<any>(`${environment.apiUrl}/addresses/by-user/${userId}`);
}

  getMyOrders(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(this.baseUrl);
  }

  getOrderById(orderId: number): Observable<OrderDetail> {
    return this.http.get<OrderDetail>(`${this.baseUrl}/${orderId}`);
  }

  cancelOrder(orderId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${orderId}`);
  }

  // ── Admin ─────────────────────────────────────────────────────
  getAllOrders(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(this.adminUrl);
  }

  getOrderByIdAdmin(id: number): Observable<OrderDetail> {
    return this.http.get<OrderDetail>(`${this.adminUrl}/${id}`);
  }

  updateOrderStatus(id: number, request: UpdateOrderStatusRequest): Observable<void> {
    return this.http.put<void>(`${this.adminUrl}/${id}/status`, request);
  }
}