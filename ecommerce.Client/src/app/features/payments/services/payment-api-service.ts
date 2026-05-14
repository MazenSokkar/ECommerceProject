// src/app/features/payments/services/payment-api.service.ts

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreatePaymentRequest, Payment, StripeIntentResponse } from '../models/Payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Payment`;

 createPayment(orderId: number, paymentMethod: string, transactionId?: string): Observable<Payment> {
  return this.http.post<Payment>(this.baseUrl, { 
    orderId, 
    paymentMethod,
    transactionId: transactionId ?? null,
  });
}

  getMyPayments(): Observable<Payment[]> {
    return this.http.get<Payment[]>(this.baseUrl);
  }

  getPaymentByOrder(orderId: number): Observable<Payment> {
    return this.http.get<Payment>(`${this.baseUrl}/order/${orderId}`);
  }

  createStripeIntent(orderId: number): Observable<StripeIntentResponse> {
  return this.http.post<StripeIntentResponse>(
    `${this.baseUrl}/stripe/create-intent`,
    { orderId }
  );


}
}