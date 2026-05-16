import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProductApiService } from '../../services/product-api.service';
import { ProductDetails } from '../../models/product.model';
import { AuthService } from '../../../../core/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { WishlistApiService } from '../../../wishlist/services/wishlist-api.service';

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
        this.wishlistApi.getMyWishlist().subscribe();
        this.inWishlist.set(this.wishlistApi.isInWishlist(this.productId));
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