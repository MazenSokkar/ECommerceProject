import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductApiService } from '../../../products/services/product-api.service';
import { Product } from '../../../products/models/product.model';

@Component({
    selector: 'app-merchant-inventory',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './merchant-inventory.html',
    styleUrl: './merchant-inventory.css',
})
export class MerchantInventory implements OnInit {
    private readonly api = inject(ProductApiService);

    protected readonly products = signal<Product[]>([]);
    protected readonly loading = signal(false);

    ngOnInit(): void {
        this.loading.set(true);
        this.api.getMyProducts().subscribe({
            next: res => {
                if (!res.data) return;
                this.products.set(res.data.items);
                this.loading.set(false);
            },
            error: () => this.loading.set(false),
        });
    }

    deleteProduct(id: number): void {
        this.api.delete(id).subscribe({
            next: () => {
                this.products.update(items => items.filter(p => p.id !== id));
            },
        });
    }
}