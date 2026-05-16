import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductApiService } from '../../services/product-api.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
    selector: 'app-product-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './product-form.html',
    styleUrl: './product-form.css',
})
export class ProductForm implements OnInit {
    private readonly api = inject(ProductApiService);
    private readonly fb = inject(FormBuilder);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly http = inject(HttpClient);

    protected readonly loading = signal(false);
    protected readonly isEdit = signal(false);
    private productId = signal<number | null>(null);

    protected readonly form = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(200)]],
        description: [''],
        price: [0, [Validators.required, Validators.min(0.01)]],
        stock: [0, [Validators.required, Validators.min(0)]],
        categoryId: [0, [Validators.required, Validators.min(1)]],
        imageUrl: [''],
    });

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.isEdit.set(true);
            this.productId.set(Number(id));
            this.api.getById(Number(id)).subscribe({
                next: res => {
                    if (!res.data) return;
                    this.form.patchValue(res.data);
                },
                error: () => this.loading.set(false),
            });
        }
    }

    submit(): void {
        if (this.form.invalid) return;
        this.loading.set(true);
        const value = this.form.value as any;

        const request$ = this.isEdit()
            ? this.api.update(this.productId()!, { ...value, isActive: true })
            : this.api.create(value);

        request$.subscribe({
            next: (res) => {
                this.loading.set(false);
                if (value.imageUrl && res.data) {
                    this.http.post(`${environment.apiUrl}/products/${res.data.id}/images`, {
                        imageUrl: value.imageUrl,
                        isPrimary: true,
                        sortOrder: 0
                    }).subscribe();
                }
                this.router.navigate(['/app/merchant/inventory']);
            },
            error: () => this.loading.set(false),
        });
    }
}