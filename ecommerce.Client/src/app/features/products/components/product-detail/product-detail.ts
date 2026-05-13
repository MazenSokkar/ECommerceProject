import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductApiService } from '../../services/product-api.service';
import { ProductDetails } from '../../models/product.model';
import { CommonModule, DecimalPipe } from '@angular/common';


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

    protected readonly product = signal<ProductDetails | null>(null);
    protected readonly loading = signal(false);
    protected readonly selectedImage = signal<string | null>(null);

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
}