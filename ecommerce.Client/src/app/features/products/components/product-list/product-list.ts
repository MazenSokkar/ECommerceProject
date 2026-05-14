import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductApiService } from '../../services/product-api.service';
import { Product, ProductFilter } from '../../models/product.model';
import { CategoryApiService } from '../../../categories/services/category-api.service';
import { Category } from '../../../categories/models/category.model';

@Component({
    selector: 'app-product-list',
    standalone: true,
    imports: [CommonModule, RouterLink, FormsModule],
    templateUrl: './product-list.html',
    styleUrl: './product-list.css',
})
export class ProductList implements OnInit {
    private readonly api = inject(ProductApiService);
    private readonly route = inject(ActivatedRoute);

    protected readonly products = signal<Product[]>([]);
    protected readonly total = signal(0);
    protected readonly loading = signal(false);
    protected readonly search = signal('');
    protected readonly sortBy = signal<'newest' | 'price_asc' | 'price_desc'>('newest');
    protected readonly page = signal(1);
    protected readonly pageSize = 20;

    protected readonly totalPages = computed(() =>
        Math.ceil(this.total() / this.pageSize)
    );
    private readonly categoryApi = inject(CategoryApiService);
    protected readonly categories = signal<Category[]>([]);
    protected readonly selectedCategory = signal<number | null>(null);

    ngOnInit(): void {
        const sortParam = this.route.snapshot.queryParamMap.get('sortBy');
        if (sortParam === 'newest' || sortParam === 'price_asc' || sortParam === 'price_desc') {
            this.sortBy.set(sortParam);
        }
        this.loadProducts();
    }

    loadProducts(): void {
        this.loading.set(true);
        const filter: ProductFilter = {
            search: this.search(),
            categoryId: this.selectedCategory() ?? undefined,
            page: this.page(),
            pageSize: this.pageSize,
            sortBy: this.sortBy(),
        };


        this.api.getAll(filter).subscribe({
            next: res => {
                if (!res.data) return;
                this.products.set(res.data.items);
                this.total.set(res.data.total);
                this.loading.set(false);
            },
            error: () => this.loading.set(false),
        });
        this.categoryApi.getAll().subscribe({
            next: res => {
                if (res.data) this.categories.set(res.data);
            }
        });
    }

    onSearch(value: string): void {
        this.search.set(value);
        this.page.set(1);
        this.loadProducts();
    }

    onSortChange(value: 'newest' | 'price_asc' | 'price_desc'): void {
        this.sortBy.set(value);
        this.page.set(1);
        this.loadProducts();
    }

    nextPage(): void {
        if (this.page() < this.totalPages()) {
            this.page.update(p => p + 1);
            this.loadProducts();
        }
    }

    prevPage(): void {
        if (this.page() > 1) {
            this.page.update(p => p - 1);
            this.loadProducts();
        }
    }
    onCategoryChange(categoryId: number | null): void {
        this.selectedCategory.set(categoryId);
        this.page.set(1);
        this.loadProducts();
    }
}