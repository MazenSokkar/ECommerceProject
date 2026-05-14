import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductApiService } from '../../services/product-api.service';
import { ProductDetails } from '../../models/product.model';
import { CommonModule, DecimalPipe } from '@angular/common';
import { CartApiService } from '../../../cart/services/cart-api-service';
import { ToastService } from '../../../../core/services/toast.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ROLES } from '../../../../shared/models/roles.model';


@Component({
    selector: 'app-product-detail',
    standalone: true,
    imports: [CommonModule, RouterLink, DecimalPipe],
    templateUrl: './product-detail.html',
    styleUrl: './product-detail.css',
})
export class ProductDetail implements OnInit {
    private readonly api = inject(ProductApiService);
    private readonly route = inject(ActivatedRoute);
    private readonly cartApi = inject(CartApiService);
private readonly toast = inject(ToastService);
private readonly auth = inject(AuthService);

    protected readonly product = signal<ProductDetails | null>(null);
    protected readonly loading = signal(false);
    protected readonly selectedImage = signal<string | null>(null);
    protected readonly addingToCart = signal(false);
protected readonly quantity = signal(1);
protected readonly isCustomer = computed(() => this.auth.hasRole(ROLES.Customer));

    ngOnInit(): void {
        const id = Number(this.route.snapshot.paramMap.get('id'));
        this.loading.set(true);
        this.api.getById(id).subscribe({
            next: res => {
                if (!res.data) return;
                this.product.set(res.data);
                this.selectedImage.set(res.data.images[0] ?? null);
                this.loading.set(false);
            },
            error: () => this.loading.set(false),
        });
    }

    selectImage(url: string): void {
        this.selectedImage.set(url);
    }
    protected increaseQty(): void {
  const max = this.product()?.stock ?? 1;
  if (this.quantity() < max) this.quantity.update(q => q + 1);
}

protected decreaseQty(): void {
  if (this.quantity() > 1) this.quantity.update(q => q - 1);
}

protected addToCart(): void {
  const p = this.product();
  if (!p || p.stock === 0) return;
  this.addingToCart.set(true);
  this.cartApi.addItem({ productId: p.id, quantity: this.quantity() }).subscribe({
    next: () => {
      this.addingToCart.set(false);
      this.toast.success('Added to Cart', `${p.name} added to your cart.`);
    },
    error: () => {
      this.addingToCart.set(false);
      this.toast.error('Failed', 'Could not add item to cart.');
    },
  });
}
}