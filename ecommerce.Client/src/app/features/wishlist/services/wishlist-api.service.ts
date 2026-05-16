import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { AddToWishlistRequest, Wishlist } from '../models/wishlist.model';

@Injectable({ providedIn: 'root' })
export class WishlistApiService {
    private readonly http = inject(HttpClient);
    private readonly base = `${environment.apiUrl}/wishlist`;

    readonly wishlistProductIds = signal<number[]>([]);

    getMyWishlist(): Observable<ApiResponse<Wishlist>> {
        return this.http.get<ApiResponse<Wishlist>>(this.base).pipe(
            tap(res => {
                if (res.data) {
                    this.wishlistProductIds.set(res.data.items.map(i => i.productId));
                }
            })
        );
    }

    addItem(request: AddToWishlistRequest): Observable<ApiResponse<Wishlist>> {
        return this.http.post<ApiResponse<Wishlist>>(`${this.base}/items`, request).pipe(
            tap(res => {
                if (res.data) {
                    this.wishlistProductIds.set(res.data.items.map(i => i.productId));
                }
            })
        );
    }

    removeItem(productId: number): Observable<ApiResponse<null>> {
        return this.http.delete<ApiResponse<null>>(`${this.base}/items/${productId}`).pipe(
            tap(() => {
                this.wishlistProductIds.update(ids => ids.filter(id => id !== productId));
            })
        );
    }

    isInWishlist(productId: number): boolean {
        return this.wishlistProductIds().includes(productId);
    }
    toggle(productId: number): Observable<any> {
        if (this.isInWishlist(productId)) {
            return this.removeItem(productId);
        }
        return this.addItem({ productId });
    }
}