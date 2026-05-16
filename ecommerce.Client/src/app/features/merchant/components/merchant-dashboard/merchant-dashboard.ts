import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductApiService } from '../../../products/services/product-api.service';
import { Product } from '../../../products/models/product.model';

@Component({
    selector: 'app-merchant-dashboard',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './merchant-dashboard.html',
    styleUrl: './merchant-dashboard.css',
})
export class MerchantDashboard implements OnInit {
    private readonly api = inject(ProductApiService);

    protected readonly products = signal<Product[]>([]);
    protected readonly loading = signal(false);

    protected readonly totalProducts = signal(0);
    protected readonly outOfStock = signal(0);
    protected readonly lowStock = signal(0);

    ngOnInit(): void {
        this.loading.set(true);
        this.api.getMyProducts().subscribe({
            next: res => {
                if (!res.data) return;
                const items = res.data;
                this.products.set(items);
                this.totalProducts.set(items.length);
                this.outOfStock.set(items.filter(p => p.stock === 0).length);
                this.lowStock.set(items.filter(p => p.stock > 0 && p.stock <= 5).length);
                this.loading.set(false);
            },
            error: () => this.loading.set(false),
        });
    }
}