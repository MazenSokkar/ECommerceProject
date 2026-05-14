// src/app/features/payments/models/Payment.model.ts

export type PaymentMethod = 'CreditCard' | 'PayPal' | 'CashOnDelivery' | 'Wallet';
export type PaymentStatus = 'Pending' | 'Completed' | 'Failed' | 'Refunded';

export interface Payment {
  id: number;
  orderId: number;
  orderNumber?: string;
  method: PaymentMethod;
  status: PaymentStatus;
  amount: number;
  transactionId?: string;
  paidAt?: string;
  createdAt: string;
}

export interface CreatePaymentRequest {
  orderId: number;
  paymentMethod: string;
  transactionId?: string;
}

export interface StripeIntentResponse {
  clientSecret: string;
  publishableKey: string;
}