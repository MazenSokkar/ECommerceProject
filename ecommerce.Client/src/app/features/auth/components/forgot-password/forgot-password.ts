import { Component, ViewChild, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../services/auth-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OtpInput } from '../../../../shared/components/otp-input/otp-input';
import { Card } from '../../../../shared/components/card/card';
import { FormField } from '../../../../shared/components/form-field/form-field';
import { PasswordInput } from '../../../../shared/components/password-input/password-input';
import { Button } from '../../../../shared/components/button/button';
import { ResendCode } from '../../../../shared/components/resend-code/resend-code';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, OtpInput, Card, FormField, PasswordInput, Button, ResendCode],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPassword {
  private readonly authApi = inject(AuthApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  @ViewChild(OtpInput) private readonly otpInput!: OtpInput;
  @ViewChild(ResendCode) private readonly resendCode!: ResendCode;

  protected readonly step = signal<1 | 2 | 3>(1);
  protected readonly loading = signal(false);
  protected readonly hasOtpError = signal(false);

  protected savedEmail = '';
  protected savedOtp = '';
  protected otpReady = false;

  protected readonly emailForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
  });

  protected readonly passwordForm = new FormGroup({
    newPassword: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(8)],
    }),
    confirmPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  protected get passwordsMatch(): boolean {
    const { newPassword, confirmPassword } = this.passwordForm.value;
    return newPassword === confirmPassword;
  }

  protected onEmailSubmit(): void {
    this.emailForm.markAllAsTouched();
    if (this.emailForm.invalid || this.loading()) return;
    this.loading.set(true);
    const email = this.emailForm.value.email!;

    this.authApi.forgotPassword({ email }).subscribe({
      next: () => {
        this.loading.set(false);
        this.savedEmail = email;
        this.toast.info('Info', 'If your email is registered, a code has been sent.');
        this.step.set(2);
        setTimeout(() => this.resendCode?.start());
      },
      error: () => {
        this.loading.set(false);
        // Always advance — don't reveal whether the email exists
        this.savedEmail = email;
        this.step.set(2);
        setTimeout(() => this.resendCode?.start());
      },
    });
  }

  protected onOtpChange(value: string): void {
    this.savedOtp = value;
    this.otpReady = value.length === 6;
    this.hasOtpError.set(false);
  }

  protected onOtpSubmit(): void {
    if (this.savedOtp.length !== 6 || this.loading()) return;
    this.step.set(3);
  }

  protected onResend(): void {
    this.authApi.forgotPassword({ email: this.savedEmail }).subscribe({
      next: () => {
        this.toast.info('Info', 'Code resent.');
        this.resendCode.start();
      },
      error: () => {
        this.resendCode.start();
      },
    });
  }

  protected onPasswordSubmit(): void {
    this.passwordForm.markAllAsTouched();
    if (this.passwordForm.invalid || !this.passwordsMatch || this.loading()) return;
    this.loading.set(true);

    this.authApi
      .resetPassword({
        email: this.savedEmail,
        code: this.savedOtp,
        newPassword: this.passwordForm.value.newPassword!,
      })
      .subscribe({
        next: (res) => {
          this.loading.set(false);
          if (res.isSuccess) {
            this.toast.success('Success', 'Password reset successfully. You can now sign in.');
            this.router.navigate(['/login']);
          }
        },
        error: (err) => {
          this.loading.set(false);
          const detail: string = err.error?.detail ?? err.error?.title ?? '';
          if (
            detail.toLowerCase().includes('code') ||
            detail.toLowerCase().includes('otp') ||
            detail.toLowerCase().includes('invalid')
          ) {
            this.step.set(1);
          }
        },
      });
  }

  protected goBack(): void {
    if (this.step() > 1) this.step.update((s) => (s - 1) as 1 | 2 | 3);
  }
}
