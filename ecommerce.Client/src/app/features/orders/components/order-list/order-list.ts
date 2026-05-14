// src/app/features/orders/components/order-list/order-list.ts

import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
;
import { ROLES } from '../../../../shared/models/roles.model';
import { AuthService } from '../../../../core/services/auth.service';
import { OrderApiService } from '../../services/order-api-service';
import { OrderStatus, OrderSummary } from '../../models/Order.model';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-list.html',
  styleUrl: './order-list.css',
})
export class OrderList implements OnInit {
  private readonly api    = inject(OrderApiService);
  private readonly router = inject(Router);
  private readonly auth   = inject(AuthService);

  protected readonly orders     = signal<OrderSummary[]>([]);
  protected readonly isLoading  = signal(false);
  protected readonly error      = signal<string | null>(null);

  protected readonly isAdmin = computed(() => this.auth.hasRole(ROLES.Admin));

  protected readonly selectedStatus = signal<'all' | OrderStatus>('all');

  protected readonly statusOptions: { value: 'all' | OrderStatus; label: string }[] = [
    { value: 'all',        label: 'All Orders'  },
    { value: 'Pending',    label: 'Pending'     },
    { value: 'Confirmed',  label: 'Confirmed'   },
    { value: 'Processing', label: 'Processing'  },
    { value: 'Shipped',    label: 'Shipped'     },
    { value: 'Delivered',  label: 'Delivered'   },
    { value: 'Cancelled',  label: 'Cancelled'   },
  ];

  protected readonly filteredOrders = computed(() => {
    const status = this.selectedStatus();
    if (status === 'all') return this.orders();
    return this.orders().filter(o => o.status === status);
  });

  ngOnInit(): void {
    this.loadOrders();
  }

  protected loadOrders(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const call = this.isAdmin()
      ? this.api.getAllOrders()
      : this.api.getMyOrders();

    call.subscribe({
      next: res => {
        this.orders.set(res ?? []);
        this.isLoading.set(false);
      },
      error: err => {
        this.error.set(err?.message ?? 'Failed to load orders.');
        this.isLoading.set(false);
      },
    });
  }

  protected filterBy(status: 'all' | OrderStatus): void {
    this.selectedStatus.set(status);
  }

  protected viewOrder(id: number): void {
    this.router.navigate(['/app/orders', id]);
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
    return map[status] ?? 'status-pending';
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
    return map[status] ?? '🕐';
  }

  protected trackById(_: number, item: OrderSummary): number {
    return item.id;
  }
}