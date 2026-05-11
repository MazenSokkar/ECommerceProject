import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../services/auth-api.service';
import { AuthService } from '../../../../core/services/auth.service';
import { UserProfile } from '../../models/auth.model';
import { Card } from '../../../../shared/components/card/card';
import { TabSwitcher } from '../../../../shared/components/tab-switcher/tab-switcher';
import { FormField } from '../../../../shared/components/form-field/form-field';
import { PasswordInput } from '../../../../shared/components/password-input/password-input';
import { AlertBanner } from '../../../../shared/components/alert-banner/alert-banner';
import { Button } from '../../../../shared/components/button/button';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, Card, TabSwitcher, FormField, PasswordInput, AlertBanner, Button],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private readonly authApi = inject(AuthApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly loginMode = signal<'email' | 'phone'>('email');
  protected readonly loading = signal(false);
  protected readonly failedAttempts = signal(0);
  protected readonly lockedUntil = signal<Date | null>(null);

  protected readonly loginTabs = [
    { value: 'email', label: 'Email' },
    { value: 'phone', label: 'Phone' },
  ];

  protected readonly form = new FormGroup({
    identifier: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    rememberMe: new FormControl(false, { nonNullable: true }),
  });

  protected setMode(mode: 'email' | 'phone'): void {
    this.loginMode.set(mode);
    const identifier = this.form.get('identifier');

    if (identifier) {
      const validators =
        mode === 'email'
          ? [Validators.required, Validators.email]
          : [Validators.required, Validators.pattern(/^(10|11|12|15)\d{8}$/)];

      identifier.setValidators(validators);
      identifier.setValue('');
      identifier.markAsUntouched();
      identifier.updateValueAndValidity();
    }
  }

  protected isLocked(): boolean {
    const until = this.lockedUntil();
    return until !== null && new Date() < until;
  }

  protected onSubmit(): void {
    if (this.form.invalid || this.loading() || this.isLocked()) return;

    this.loading.set(true);

    const raw = this.form.value.identifier!;
    const identifier = this.loginMode() === 'phone' ? `+20${raw}` : raw;

    this.authApi
      .login({ identifier, password: this.form.value.password! })
      .subscribe({
        next: (res) => {
          this.loading.set(false);
          if (res.isSuccess && res.data) {
            this.failedAttempts.set(0);
            const profile: UserProfile = {
              firstName: res.data.firstName,
              lastName: res.data.lastName,
              email: res.data.email,
              roles: res.data.roles,
            };
            this.auth.setSession(res.data.token, profile);
            this.router.navigate(['/app']);
          }
        },
        error: (err) => {
          this.loading.set(false);
          this.failedAttempts.update((n) => n + 1);

          if (
            err.status === 429 ||
            (err.status === 400 && err.error?.detail?.toLowerCase().includes('lockout'))
          ) {
            this.lockedUntil.set(new Date(Date.now() + 15 * 60 * 1000));
            return;
          }

          const detail: string = err.error?.detail ?? err.error?.title ?? '';

          if (
            detail.toLowerCase().includes('confirm') ||
            detail.toLowerCase().includes('not confirmed')
          ) {
            this.router.navigate(['/confirm-email'], {
              queryParams: { email: identifier },
            });
          }
        },
      });
  }
}
