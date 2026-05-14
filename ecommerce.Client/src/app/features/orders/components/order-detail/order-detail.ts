// src/app/features/orders/components/order-detail/order-detail.ts

import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  OrderDetail as OrderDetailModel,
  OrderStatus,
  UpdateOrderStatusRequest,
} from '../../models/Order.model';
import { ROLES } from '../../../../shared/models/roles.model';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OrderApiService } from '../../services/order-api-service';
import { ShippingApiService } from '../../../shipping/services/shipping-api-service';
import { Shipment, ShippingStatus } from '../../../shipping/models/Shipping.model';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule ,RouterLink ],
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.css',
})
export class OrderDetailComponent implements OnInit {
  private readonly api    = inject(OrderApiService);
  private readonly route  = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly auth   = inject(AuthService);
  private readonly toast  = inject(ToastService);
  private readonly shippingApi = inject(ShippingApiService);

protected readonly shipment = signal<Shipment | null>(null);
protected readonly isCreatingShipment = signal(false);
protected readonly isUpdatingShipment = signal(false);
protected readonly selectedShipmentStatus = signal<ShippingStatus>('Preparing');

readonly shipmentStatusOptions: ShippingStatus[] = [
  'Preparing', 'Shipped', 'InTransit', 'Delivered', 'Returned'
];

private loadShipment(orderId: number): void {
  this.shippingApi.getShipmentAdmin(orderId).subscribe({
    next: res => this.shipment.set(res),
    error: () => this.shipment.set(null), 
  });
}

protected createShipment(): void {
  const id = this.orderData()?.id;
  if (!id) return;
  this.isCreatingShipment.set(true);
  this.shippingApi.createShipment(id).subscribe({
    next: () => {
      this.toast.success('Shipment Created', 'Shipment created successfully.');
      this.loadShipment(id);
      this.isCreatingShipment.set(false);
    },
    error: () => {
      this.toast.error('Error', 'Could not create shipment.');
      this.isCreatingShipment.set(false);
    },
  });
}

protected updateShipmentStatus(): void {
  const id = this.orderData()?.id;
  if (!id) return;
  this.isUpdatingShipment.set(true);
  this.shippingApi.updateShipmentStatus(id, this.selectedShipmentStatus()).subscribe({
    next: () => {
      this.toast.success('Shipment Updated', `Status changed to ${this.selectedShipmentStatus()}.`);
      this.loadShipment(id);
      this.isUpdatingShipment.set(false);
    },
    error: () => {
      this.toast.error('Error', 'Could not update shipment status.');
      this.isUpdatingShipment.set(false);
    },
  });
}
  protected readonly orderData         = signal<OrderDetailModel | null>(null);
  protected readonly isLoading         = signal(false);
  protected readonly error             = signal<string | null>(null);
  protected readonly isCancelling      = signal(false);
  protected readonly isUpdatingStatus  = signal(false);

  protected readonly isAdmin   = computed(() => this.auth.hasRole(ROLES.Admin));
  protected readonly canCancel = computed(() =>
    !this.isAdmin() && this.orderData()?.status === 'Pending'
  );

  protected readonly canPay = computed(() =>
  !this.isAdmin() && this.orderData()?.status === 'Pending'
);
protected goToPayment(): void {
  const order = this.orderData();
  if (!order) return;
  this.router.navigate(['/app/payment'], {
    queryParams: {
      orderId: order.id,
      orderNumber: order.orderNumber,
      total: order.totalAmount,
    }
  });
}

  protected readonly statusOptions: OrderStatus[] = [
    'Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled',
  ];

  protected readonly selectedNewStatus = signal<OrderStatus>('Pending');

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadOrder(id);
  }

  private loadOrder(id: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    const call = this.isAdmin()
      ? this.api.getOrderByIdAdmin(id)
      : this.api.getOrderById(id);

    call.subscribe({
      next: (res: OrderDetailModel) => {
        this.orderData.set(res);
        this.selectedNewStatus.set(res.status);
        this.isLoading.set(false);
        if (this.isAdmin()) this.loadShipment(res.id);
      },
      error: (err: any) => {
        this.error.set(err?.message ?? 'Failed to load order.');
        this.isLoading.set(false);
      },
    });
  }

  protected cancelOrder(): void {
    const id = this.orderData()?.id;
    if (!id) return;

    this.isCancelling.set(true);
    this.api.cancelOrder(id).subscribe({
      next: () => {
        this.toast.success('Order Cancelled', 'Your order has been cancelled successfully.');
        this.router.navigate(['/app/orders']);
      },
      error: (err: any) => {
        this.toast.error('Cancel Failed', err?.message ?? 'Failed to cancel order.');
        this.isCancelling.set(false);
      },
    });
  }

  protected updateStatus(): void {
    const id = this.orderData()?.id;
    if (!id) return;

    const newStatus = this.selectedNewStatus();
    this.isUpdatingStatus.set(true);

    const request: UpdateOrderStatusRequest = { status: newStatus };

    this.api.updateOrderStatus(id, request).subscribe({
      next: () => {
        this.toast.success('Status Updated', `Order status changed to ${newStatus}.`);
        this.orderData.update((o): OrderDetailModel | null =>
          o ? { ...o, status: newStatus } : null
        );
        this.isUpdatingStatus.set(false);
      },
      error: (err: any) => {
        this.toast.error('Update Failed', err?.message ?? 'Failed to update status.');
        this.isUpdatingStatus.set(false);
      },
    });
  }

  protected goBack(): void {
    this.router.navigate(['/app/orders']);
  }

  protected getStatusClass(status: OrderStatus): string {
    const map: Record<OrderStatus, string> = {
      Pending:    'status-pending',
      Confirmed:  'status-confirmed',
      Processing: 'status-processing',
      Shipped:    'status-shipped',
      Delivered:  'status-delivered',
      Cancelled:  'status-cancelled',
    };
    return map[status];
  }

  protected getStatusIcon(status: OrderStatus): string {
    const map: Record<OrderStatus, string> = {
      Pending:    '🕐',
      Confirmed:  '✅',
      Processing: '⚙️',
      Shipped:    '🚚',
      Delivered:  '📦',
      Cancelled:  '❌',
    };
    return map[status];
  }
}