import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { Product } from '../../../products/models/product.model';

@Component({
    selector: 'app-admin-products',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './admin-products.html',
    styleUrl: './admin-products.css',
})
export class AdminProducts implements OnInit {
    private readonly http = inject(HttpClient);

    protected readonly products = signal<Product[]>([]);
    protected readonly loading = signal(false);

    ngOnInit(): void {
        this.loading.set(true);
        this.http.get<any>(`${environment.apiUrl}/products?page=1&pageSize=100&sortBy=newest`).subscribe({
            next: res => {
                if (res.data) this.products.set(res.data.items);
                this.loading.set(false);
            },
            error: () => this.loading.set(false),
        });
    }

    deleteProduct(id: number): void {
        this.http.delete(`${environment.apiUrl}/products/admin/${id}`).subscribe({
            next: () => this.products.update(p => p.filter(x => x.id !== id)),
        });

    }
}