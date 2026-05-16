import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductApiService } from '../../services/product-api.service';
import { ProductDetails } from '../../models/product.model';
import { AuthService } from '../../../../core/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { WishlistApiService } from '../../../wishlist/services/wishlist-api.service';
import { CartApiService } from '../../../cart/services/cart-api-service';
import { ToastService } from '../../../../core/services/toast.service';
import { ROLES } from '../../../../shared/models/roles.model';

interface Review {
  id: number;
  productId: number;
  userId: number;
  userName: string;
  rating: number;
  comment?: string;
  createdAt: string;
}

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, DecimalPipe, DatePipe],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.css',
})
export class ProductDetail implements OnInit {
  private readonly api = inject(ProductApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly auth = inject(AuthService);
  private readonly http = inject(HttpClient);
  private readonly wishlistApi = inject(WishlistApiService);
  private readonly cartApi = inject(CartApiService);
  private readonly toast = inject(ToastService);

  protected readonly product = signal<ProductDetails | null>(null);
  protected readonly loading = signal(false);
  protected readonly selectedImage = signal<string | null>(null);
  protected readonly reviews = signal<Review[]>([]);
  protected readonly selectedRating = signal(0);
  protected readonly reviewComment = signal('');
  protected readonly submittingReview = signal(false);
  protected readonly isAuthenticated = computed(() => this.auth.isAuthenticated());
  protected readonly inWishlist = signal(false);
  protected readonly wishlistLoading = signal(false);
  protected readonly addingToCart = signal(false);
  protected readonly quantity = signal(1);
  protected readonly isCustomer = computed(() => this.auth.hasRole(ROLES.Customer));

  private productId = 0;

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.loading.set(true);

    this.api.getById(this.productId).subscribe({
      next: res => {
        if (!res.data) return;
        this.product.set(res.data);
        this.selectedImage.set(res.data.images[0] ?? null);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });

    this.loadReviews();

    this.wishlistApi.getMyWishlist().subscribe({
      next: () => {
        this.inWishlist.set(this.wishlistApi.isInWishlist(this.productId));
      }
    });
  }

  loadReviews(): void {
    this.http.get<any>(`${environment.apiUrl}/reviews/product/${this.productId}`).subscribe({
      next: res => {
        if (res.data) this.reviews.set(res.data);
      }
    });
  }

  selectImage(url: string): void {
    this.selectedImage.set(url);
  }

  setRating(rating: number): void {
    this.selectedRating.set(rating);
  }

  toggleWishlist(): void {
    this.wishlistLoading.set(true);
    this.wishlistApi.toggle(this.productId).subscribe({
      next: () => {
        this.inWishlist.set(this.wishlistApi.isInWishlist(this.productId));
        this.wishlistLoading.set(false);
      },
      error: () => this.wishlistLoading.set(false),
    });
  }

  increaseQty(): void {
    const max = this.product()?.stock ?? 1;
    if (this.quantity() < max) this.quantity.update(q => q + 1);
  }

  decreaseQty(): void {
    if (this.quantity() > 1) this.quantity.update(q => q - 1);
  }

  addToCart(): void {
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

  submitReview(): void {
    if (this.selectedRating() === 0) return;
    this.submittingReview.set(true);

    this.http.post<any>(`${environment.apiUrl}/reviews`, {
      productId: this.productId,
      rating: this.selectedRating(),
      comment: this.reviewComment(),
    }).subscribe({
      next: res => {
        if (res.data) {
          this.reviews.update(r => [res.data, ...r]);
          this.selectedRating.set(0);
          this.reviewComment.set('');
        }
        this.submittingReview.set(false);
      },
      error: () => this.submittingReview.set(false),
    });
  }
}