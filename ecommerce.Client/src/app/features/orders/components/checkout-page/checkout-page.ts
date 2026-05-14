// src/app/features/orders/components/checkout-page/checkout-page.ts

import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { ToastService } from '../../../../core/services/toast.service';
import { OrderApiService } from '../../services/order-api-service';
import { CartApiService } from '../../../cart/services/cart-api-service';
import { Cart } from '../../../cart/models/Cart.model';
import { AuthService } from '../../../../core/services/auth.service';

interface Address {
  id: number;
  locationName: string;
  cityId: number;
  stateProvinceId: number;
  countryId: number;
}

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './checkout-page.html',
  styleUrl: './checkout-page.css',
})
export class CheckoutPage implements OnInit {
  private readonly orderApi = inject(OrderApiService);
  private readonly cartApi = inject(CartApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  protected readonly cart = signal<Cart | null>(null);
  protected readonly addresses = signal<Address[]>([]);
  protected readonly isLoadingCart = signal(false);
  protected readonly isLoadingAddresses = signal(false);
  protected readonly isPlacingOrder = signal(false);

  protected readonly selectedAddressId = signal<number | null>(null);
  protected readonly notes = signal('');

  protected readonly canPlaceOrder = computed(() =>
    this.selectedAddressId() !== null &&
    !this.isPlacingOrder() &&
    (this.cart()?.items?.length ?? 0) > 0
  );

  protected readonly totalAmount = computed(() => this.cart()?.total ?? 0);
  protected readonly itemsCount = computed(() => this.cart()?.totalItems ?? 0);

  ngOnInit(): void {
    this.loadCart();
    this.loadAddresses();
  }

  private loadCart(): void {
    this.isLoadingCart.set(true);
    this.cartApi.getCart().subscribe({
      next: res => {
        this.cart.set(res.data);
        this.isLoadingCart.set(false);
        if (!res.data || res.data.items.length === 0) {
          this.toast.error('Empty Cart', 'Your cart is empty. Add items before checkout.');
          this.router.navigate(['/products']);
        }
      },
      error: () => this.isLoadingCart.set(false),
    });
  }



private loadAddresses(): void {
  const userId = this.authService.getUserId();

  if (!userId) {
    this.toast.error('Error', 'User not found');
    return;
  }

  this.isLoadingAddresses.set(true);

  this.orderApi.getMyAddresses(userId).subscribe({
    next: (res: any) => {
      const data = res.data ?? [];

      this.addresses.set(data);

      if (data.length > 0) {
        this.selectedAddressId.set(data[0].id);
      }

      this.isLoadingAddresses.set(false);
    },
    error: () => this.isLoadingAddresses.set(false),
  });
}

  protected selectAddress(id: number): void {
    this.selectedAddressId.set(id);
  }

  protected placeOrder(): void {
    if (!this.canPlaceOrder()) return;

    this.isPlacingOrder.set(true);
    this.orderApi.placeOrder({
      shippingAddressId: this.selectedAddressId()!,
      notes: this.notes(),
    }).subscribe({
next: (order: any) => {
  this.isPlacingOrder.set(false);
  this.toast.success('Order Placed!', `Order ${order.orderNumber} placed successfully.`);
  this.router.navigate(['/app/payment'], {
    queryParams: {
      orderId: order.id,
      orderNumber: order.orderNumber,
      total: order.totalAmount ?? order.total ?? order.subtotal ?? 0,
    }
  });
},
      error: (err) => {
        this.isPlacingOrder.set(false);
        this.toast.error('Order Failed', 'Could not place order. Please try again.');
        console.log(err)
      },
    });
  }
}