import { Component, ViewChild, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../services/auth-api.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { OtpInput } from '../../../../shared/components/otp-input/otp-input';
import { UserProfile } from '../../models/auth.model';
import { Card } from '../../../../shared/components/card/card';
import { IconHeader } from '../../../../shared/components/icon-header/icon-header';
import { Button } from '../../../../shared/components/button/button';
import { ResendCode } from '../../../../shared/components/resend-code/resend-code';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [OtpInput, RouterLink, Card, IconHeader, Button, ResendCode],
  templateUrl: './confirm-email.html',
  styleUrl: './confirm-email.css',
})
export class ConfirmEmail {
  private readonly authApi = inject(AuthApiService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  @ViewChild(OtpInput) private readonly otpInput!: OtpInput;
  @ViewChild(ResendCode) private readonly resendCode!: ResendCode;

  protected readonly email = signal(this.route.snapshot.queryParamMap.get('email') ?? '');
  protected readonly loading = signal(false);
  protected readonly resendLoading = signal(false);
  protected readonly hasError = signal(false);
  protected readonly otpReady = signal(false);

  private otpValue = '';

  protected onOtpChange(value: string): void {
    this.otpValue = value;
    this.otpReady.set(value.length === 6);
    this.hasError.set(false);
  }

  protected onSubmit(): void {
    if (this.otpValue.length !== 6 || this.loading()) return;
    this.loading.set(true);
    this.hasError.set(false);

    this.authApi.confirmEmail({ email: this.email(), code: this.otpValue }).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.isSuccess && res.data) {
          const profile: UserProfile = {
            firstName: res.data.firstName,
            lastName: res.data.lastName,
            email: res.data.email,
            roles: res.data.roles,
          };
          this.auth.setSession(res.data.token, profile);
          this.toast.success('Success', 'Email confirmed! Welcome to TechShop 🎉');
          this.router.navigate(['/app']);
        }
      },
      error: () => {
        this.loading.set(false);
        this.hasError.set(true);
        this.otpInput?.shake();
        this.otpInput?.clear();
      },
    });
  }

  protected onResend(): void {
    this.resendLoading.set(true);
    this.authApi.resendConfirmation(this.email()).subscribe({
      next: () => {
        this.resendLoading.set(false);
        this.toast.info('Info', 'Confirmation code resent. Check your inbox.');
        this.resendCode.start();
      },
      error: () => {
        this.resendLoading.set(false);
      },
    });
  }
}
