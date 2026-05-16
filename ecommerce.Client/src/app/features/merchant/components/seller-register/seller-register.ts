import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
    selector: 'app-seller-register',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './seller-register.html',
    styleUrl: './seller-register.css',
})
export class SellerRegister {
    private readonly fb = inject(FormBuilder);
    private readonly http = inject(HttpClient);
    private readonly router = inject(Router);
    private readonly toast = inject(ToastService);

    protected readonly loading = signal(false);

    protected readonly form = this.fb.group({
        storeName: ['', [Validators.required, Validators.maxLength(150)]],
        description: [''],
        storeLogo: [''],
    });

    submit(): void {
        if (this.form.invalid) return;
        this.loading.set(true);

        this.http.post<any>(`${environment.apiUrl}/merchants/register`, this.form.value).subscribe({
            next: () => {
                this.toast.success('Success', 'Seller registration submitted! Waiting for approval.');
                this.router.navigate(['/app']);
                this.loading.set(false);
            },
            error: () => {
                this.toast.error('Error', 'Something went wrong.');
                this.loading.set(false);
            },
        });
    }
}