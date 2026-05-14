// src/app/features/shipping/components/shipping-tracking/shipping-tracking.ts

import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { Shipment, ShippingStatus } from '../../models/Shipping.model';
import { ToastService } from '../../../../core/services/toast.service';
import { ShippingApiService } from '../../services/shipping-api-service';

interface TrackingStep {
  status: ShippingStatus;
  label: string;
  icon: string;
  description: string;
}

@Component({
  selector: 'app-shipping-tracking',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './shipping-tracking.html',
  styleUrl: './shipping-tracking.css',
})
export class ShippingTracking implements OnInit {
  private readonly shippingApi = inject(ShippingApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  protected readonly shipment = signal<Shipment | null>(null);
  protected readonly isLoading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly orderId = signal<number>(0);

  readonly steps: TrackingStep[] = [
    { status: 'Preparing',  label: 'Preparing',   icon: '📦', description: 'Your order is being prepared' },
    { status: 'Shipped',    label: 'Shipped',      icon: '🚚', description: 'Your order has been shipped' },
    { status: 'InTransit',  label: 'In Transit',   icon: '🛣️', description: 'Your order is on the way' },
    { status: 'Delivered',  label: 'Delivered',    icon: '✅', description: 'Your order has been delivered' },
  ];

  protected readonly currentStepIndex = computed(() => {
    const status = this.shipment()?.status;
    if (!status) return -1;
    if (status === 'Returned') return -1;
    return this.steps.findIndex(s => s.status === status);
  });

  protected readonly isReturned = computed(() => this.shipment()?.status === 'Returned');

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('orderId'));
    if (!id) {
      this.toast.error('Error', 'Invalid order ID.');
      this.router.navigate(['/app/orders']);
      return;
    }
    this.orderId.set(id);
    this.loadShipment(id);
  }

  private loadShipment(orderId: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.shippingApi.getShipment(orderId).subscribe({
      next: res => {
        this.shipment.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('No shipment found for this order yet.');
        this.isLoading.set(false);
      },
    });
  }

  protected isStepCompleted(index: number): boolean {
    return index < this.currentStepIndex();
  }

  protected isStepActive(index: number): boolean {
    return index === this.currentStepIndex();
  }

  protected getStepClass(index: number): string {
    if (this.isReturned()) return 'step-returned';
    if (this.isStepActive(index)) return 'step-active';
    if (this.isStepCompleted(index)) return 'step-completed';
    return 'step-pending';
  }
}