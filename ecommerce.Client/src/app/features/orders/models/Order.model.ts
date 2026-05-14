// features/orders/models/order.model.ts

export type OrderStatus =
  | 'Pending'
  | 'Confirmed'
  | 'Processing'
  | 'Shipped'
  | 'Delivered'
  | 'Cancelled';

export interface OrderSummary {
  id: number;
  orderNumber: string;
  status: OrderStatus;
  totalAmount: number;
  createdAt: string;
  itemsCount: number;
}

export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}
export interface ShippingAddress {
  fullName: string;
  phone: string;
  addressLine: string;
  city: string;
  country: string;
}

export interface OrderDetail {
  id: number;
  orderNumber: string;
  status: OrderStatus;
  subtotal: number;
  shippingFee: number;
  totalAmount: number;
  createdAt: string;
shippingAddress: ShippingAddress; 
  items: OrderItem[];
}

export interface PlaceOrderRequest {
  addressId: number;
  paymentMethod: 'CreditCard' | 'PayPal' | 'CashOnDelivery' | 'Wallet';
  shippingAddressId: number;
  notes: string;
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus;
}