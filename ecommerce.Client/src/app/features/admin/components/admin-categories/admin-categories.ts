import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryApiService } from '../../../categories/services/category-api.service';
import { Category } from '../../../categories/models/category.model';

@Component({
    selector: 'app-admin-categories',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './admin-categories.html',
    styleUrl: './admin-categories.css',
})
export class AdminCategories implements OnInit {
    private readonly api = inject(CategoryApiService);
    private readonly fb = inject(FormBuilder);

    protected readonly categories = signal<Category[]>([]);
    protected readonly loading = signal(false);
    protected readonly submitting = signal(false);

    protected readonly form = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100)]],
        parentId: [null],
        imageUrl: [''],
    });

    ngOnInit(): void {
        this.loadCategories();
    }

    loadCategories(): void {
        this.loading.set(true);
        this.api.getAll().subscribe({
            next: res => {
                if (res.data) this.categories.set(res.data);
                this.loading.set(false);
            },
            error: () => this.loading.set(false),
        });
    }

    submit(): void {
        if (this.form.invalid) return;
        this.submitting.set(true);

        this.api.create(this.form.value as any).subscribe({
            next: res => {
                if (res.data) this.categories.update(c => [...c, res.data!]);
                this.form.reset();
                this.submitting.set(false);
            },
            error: () => this.submitting.set(false),
        });
    }

    deleteCategory(id: number): void {
        this.api.delete(id).subscribe({
            next: () => this.categories.update(c => c.filter(x => x.id !== id)),
        });
    }
}