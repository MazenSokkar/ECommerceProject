import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ToastService } from '../../../../core/services/toast.service';
import { CartApiService } from '../../services/cart-api-service';
import { Cart, CartItem } from '../../models/Cart.model';

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, RouterLink],
  templateUrl: './cart-page.html',
  styleUrl: './cart-page.css',
})
export class CartPage implements OnInit {

    private readonly api = inject(CartApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
 
  protected readonly cart = signal<Cart | null>(null);
  protected readonly isLoading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly removingProductId = signal<number | null>(null);
  protected readonly updatingProductId = signal<number | null>(null);
  protected readonly isClearing = signal(false);
 
 protected readonly isEmpty = computed(() => {
  const c = this.cart();
  return !c || c.items.length === 0;
});
 
  protected readonly itemsCount = computed(() => this.cart()?.totalItems ?? 0);
protected readonly totalAmount = computed(() => this.cart()?.total ?? 0);
 
  ngOnInit(): void {
    this.loadCart();
  }
 
  private loadCart(): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.api.getCart().subscribe({
      next: res => {
        this.cart.set(res.data);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load cart. Please try again.');
        this.isLoading.set(false);
      },
    });
  }
 
  protected updateQuantity(item: CartItem, delta: number): void {
    const newQty = item.quantity + delta;
    if (newQty < 1) return;
 
    this.updatingProductId.set(item.productId);
    this.api.updateItem(item.productId, { quantity: newQty }).subscribe({
      next: res => {
        this.cart.set(res.data);
        this.updatingProductId.set(null);
      },
      error: () => {
        this.toast.error('Update Failed', 'Could not update quantity.');
        this.updatingProductId.set(null);
      },
    });
  }
 
  protected removeItem(productId: number): void {
    this.removingProductId.set(productId);
    this.api.removeItem(productId).subscribe({
      next: () => {
        this.cart.update(c => {
  if (!c) return c;
  const items = c.items.filter(i => i.productId !== productId);
  const total = items.reduce((sum, i) => sum + i.subtotal, 0);
  const totalItems = items.reduce((sum, i) => sum + i.quantity, 0);
  return { ...c, items, total, totalItems };
});
        this.removingProductId.set(null);
        this.toast.success('Item Removed', 'Item removed from cart.');
      },
      error: () => {
        this.toast.error('Remove Failed', 'Could not remove item.');
        this.removingProductId.set(null);
      },
    });
  }
 
  protected clearCart(): void {
    this.isClearing.set(true);
    this.api.clearCart().subscribe({
      next: () => {
        this.cart.set(null);
        this.isClearing.set(false);
        this.toast.success('Cart Cleared', 'Your cart has been cleared.');
      },
      error: () => {
        this.toast.error('Clear Failed', 'Could not clear cart.');
        this.isClearing.set(false);
      },
    });
  }
 
protected checkout(): void {
  this.router.navigate(['/app/orders/checkout']);
}
  
}
