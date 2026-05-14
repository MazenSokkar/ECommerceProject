// src/app/features/payments/components/payment-page/payment-page.ts

import { Component, OnInit, OnDestroy, inject, signal, computed, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { loadStripe, Stripe, StripeCardElement, StripeElements } from '@stripe/stripe-js';

import { ToastService } from '../../../../core/services/toast.service';
import { PaymentMethod } from '../../models/Payment.model';
import { PaymentApiService } from '../../services/payment-api-service';

@Component({
  selector: 'app-payment-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './payment-page.html',
  styleUrl: './payment-page.css',
})
export class PaymentPage implements OnInit, OnDestroy {
  private readonly paymentApi = inject(PaymentApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly platformId = inject(PLATFORM_ID);

  // ── State ─────────────────────────────────────────────────────
  protected readonly orderId = signal<number | null>(null);
  protected readonly orderNumber = signal<string>('');
  protected readonly orderTotal = signal<number>(0);

  protected readonly selectedMethod = signal<PaymentMethod>('CreditCard');
  protected readonly isLoadingIntent = signal(false);
  protected readonly isProcessing = signal(false);
  protected readonly cardError = signal<string | null>(null);
  protected readonly cardComplete = signal(false);

  protected readonly canPay = computed(() =>
  !this.isProcessing() &&
  !this.isLoadingIntent() &&
  (this.selectedMethod() !== 'CreditCard' || this.cardComplete())
);

  readonly methods: { value: PaymentMethod; label: string; icon: string; desc: string }[] = [
    {
      value: 'CreditCard',
      label: 'Credit / Debit Card',
      icon: 'card',
      desc: 'Secure payment via Stripe',
    },
    {
      value: 'CashOnDelivery',
      label: 'Cash on Delivery',
      icon: 'cash',
      desc: 'Pay when your order arrives',
    },
    {
      value: 'Wallet',
      label: 'Wallet',
      icon: 'wallet',
      desc: 'Use your account balance',
    },
  ];

  // ── Stripe internals ──────────────────────────────────────────
  private stripe: Stripe | null = null;
  private elements: StripeElements | null = null;
  private cardElement: StripeCardElement | null = null;
  private clientSecret: string | null = null;

  // ── Lifecycle ─────────────────────────────────────────────────
  ngOnInit(): void {
    // Read query params: ?orderId=5&orderNumber=ORD-001&total=150
    const params = this.route.snapshot.queryParamMap;
    const id = Number(params.get('orderId'));
    if (!id) {
      this.toast.error('Invalid Order', 'No order ID provided.');
      this.router.navigate(['/app/orders']);
      return;
    }
    this.orderId.set(id);
    this.orderNumber.set(params.get('orderNumber') ?? '');
    this.orderTotal.set(Number(params.get('total') ?? 0));

    if (isPlatformBrowser(this.platformId)) {
      this.initStripe();
    }
  }

  ngOnDestroy(): void {
    this.cardElement?.destroy();
  }

  // ── Stripe setup ──────────────────────────────────────────────
 private async initStripe(): Promise<void> {
  this.isLoadingIntent.set(true);
  try {
    const res = await this.paymentApi.createStripeIntent(this.orderId()!).toPromise();
    this.clientSecret = res!.clientSecret;
    this.stripe = await loadStripe(res!.publishableKey);
    if (!this.stripe) throw new Error('Stripe failed to load');
    this.elements = this.stripe.elements();
  } catch {
    this.toast.error('Payment Error', 'Could not initialize payment. Try again.');
  } finally {
    this.isLoadingIntent.set(false);
    // mount بعد ما الـ @if يظهر الـ card section
    setTimeout(() => this.mountCard(), 200);
  }
}

 private mountCard(): void {
  if (!this.elements) return;

  // destroy القديم لو موجود
  this.cardElement?.destroy();
  this.cardElement = null;

  this.cardElement = this.elements.create('card', {
    style: {
      base: {
        fontSize: '15px',
        color: '#1F2937',
        fontFamily: 'Inter, sans-serif',
        '::placeholder': { color: '#9CA3AF' },
      },
      invalid: { color: '#DC2626' },
    },
    hidePostalCode: true,
  });

  // زود الـ delay لـ 300ms عشان Angular يرندر الـ @if الأول
  setTimeout(() => {
    const el = document.getElementById('stripe-card-element');
    if (el) {
      this.cardElement!.mount(el);
      this.cardElement!.on('change', event => {
        this.cardError.set(event.error?.message ?? null);
        this.cardComplete.set(event.complete);
      });
    }
  }, 300);
}

  // ── Method selection ──────────────────────────────────────────
  protected selectMethod(method: PaymentMethod): void {
    this.selectedMethod.set(method);

    if (method === 'CreditCard') {
      // Re-mount card if switching back
      setTimeout(() => this.mountCard(), 50);
    }
  }

  // ── Pay ───────────────────────────────────────────────────────
  protected async pay(): Promise<void> {
  if (!this.orderId()) return;
  this.isProcessing.set(true);

  const method = this.selectedMethod();

  if (method === 'CreditCard') {
    // لو stripe لسه null، انتظر شوية وجرب تاني
    if (!this.stripe || !this.clientSecret) {
      this.toast.error('Payment Error', 'Payment not ready yet. Please wait a moment.');
      this.isProcessing.set(false);
      return;
    }
    await this.payWithStripe();
  } else {
    this.payWithOtherMethod(method);
  }
}


private async payWithStripe(): Promise<void> {
  if (!this.stripe || !this.cardElement || !this.clientSecret) {
    this.toast.error('Payment Error', 'Stripe not initialized.');
    this.isProcessing.set(false);
    return;
  }

  const { error, paymentIntent } = await this.stripe.confirmCardPayment(this.clientSecret, {
    payment_method: { card: this.cardElement },
  });

  if (error) {
    this.cardError.set(error.message ?? 'Payment failed.');
    this.isProcessing.set(false);
    return;
  }

  if (paymentIntent?.status === 'succeeded') {
    this.paymentApi.createPayment(
      this.orderId()!,
      'CreditCard',
      paymentIntent.id
    ).subscribe({
      next: () => this.onSuccess(),
      error: (err) => {
        console.log('createPayment error:', err);
        this.onSuccess();
      },
    });
  }
}

private payWithOtherMethod(method: PaymentMethod): void {
  this.paymentApi.createPayment(this.orderId()!, method).subscribe({
    next: () => this.onSuccess(),
    error: () => {
      this.toast.error('Payment Failed', 'Could not process payment. Please try again.');
      this.isProcessing.set(false);
    },
  });
}

  private onSuccess(): void {
    this.isProcessing.set(false);
    this.toast.success('Payment Successful! 🎉', `Order ${this.orderNumber()} is confirmed.`);
    this.router.navigate(['/app/orders', this.orderId()]);
  }

  
}