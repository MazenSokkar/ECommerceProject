import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { WishlistApiService } from '../../services/wishlist-api.service';
import { WishlistItem } from '../../models/wishlist.model';

@Component({
    selector: 'app-wishlist-page',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './wishlist-page.html',
    styleUrl: './wishlist-page.css',
})
export class WishlistPage implements OnInit {
    private readonly api = inject(WishlistApiService);

    protected readonly items = signal<WishlistItem[]>([]);
    protected readonly loading = signal(false);

    ngOnInit(): void {
        this.loading.set(true);
        this.api.getMyWishlist().subscribe({
            next: res => {
                if (res.data) this.items.set(res.data.items);
                this.loading.set(false);
            },
            error: () => this.loading.set(false),
        });
    }

    removeItem(productId: number): void {
        this.api.removeItem(productId).subscribe({
            next: () => {
                this.items.update(items => items.filter(i => i.productId !== productId));
            },
        });
    }
}