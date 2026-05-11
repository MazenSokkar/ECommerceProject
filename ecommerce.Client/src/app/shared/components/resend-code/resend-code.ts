import { Component, OnDestroy, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-resend-code',
  standalone: true,
  templateUrl: './resend-code.html',
  styleUrl: './resend-code.css',
})
export class ResendCode implements OnDestroy {
  readonly cooldownSeconds = input(60);
  readonly loading = input(false);
  readonly prompt = input("Didn't receive it?");
  readonly resend = output<void>();

  protected readonly countdown = signal(0);
  private interval: ReturnType<typeof setInterval> | null = null;

  /** Call this after a successful resend API request to start the countdown. */
  start(): void {
    if (this.interval) clearInterval(this.interval);
    this.countdown.set(this.cooldownSeconds());
    this.interval = setInterval(() => {
      this.countdown.update((v) => {
        if (v <= 1) {
          clearInterval(this.interval!);
          this.interval = null;
          return 0;
        }
        return v - 1;
      });
    }, 1000);
  }

  protected onResend(): void {
    if (this.countdown() > 0 || this.loading()) return;
    this.resend.emit();
  }

  ngOnDestroy(): void {
    if (this.interval) clearInterval(this.interval);
  }
}
