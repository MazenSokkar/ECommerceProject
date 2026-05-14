
export type ShippingStatus =
  | 'Preparing'
  | 'Shipped'
  | 'InTransit'
  | 'Delivered'
  | 'Returned';

export interface Shipment {
  id: number;
  orderId: number;
  trackingNumber?: string;
  carrier?: string;
  status: ShippingStatus;
  shippedAt?: string;
  deliveredAt?: string;
}