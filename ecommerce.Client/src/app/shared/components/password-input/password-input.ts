import { Component, OnDestroy, OnInit, computed, input, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-password-input',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './password-input.html',
  styleUrl: './password-input.css',
})
export class PasswordInput implements OnInit, OnDestroy {
  readonly label = input('Password');
  readonly control = input.required<FormControl>();
  readonly placeholder = input('••••••••');
  readonly autocomplete = input<'current-password' | 'new-password'>('current-password');
  readonly required = input(false);
  readonly showStrengthMeter = input(false);

  protected readonly show = signal(false);
  private readonly _strength = signal(0);
  private _sub?: Subscription;

  protected readonly strengthColors = computed(() => {
    const s = this._strength();
    const colors = ['', 'bg-danger', 'bg-warning', 'bg-secondary', 'bg-success'];
    return Array.from({ length: 4 }, (_, i) =>
      i < s ? colors[s] : 'bg-neutral-200 dark:bg-slate-600',
    );
  });

  protected readonly strengthLabel = computed(
    () => ['', 'Weak', 'Fair', 'Good', 'Strong'][this._strength()],
  );

  protected readonly hasError = computed(
    () => this.control().invalid && this.control().touched,
  );

  protected readonly errors = computed(() => this.control().errors ?? {});

  ngOnInit(): void {
    this._strength.set(this.computeStrength(this.control().value ?? ''));
    this._sub = this.control().valueChanges.subscribe((v: string) =>
      this._strength.set(this.computeStrength(v ?? '')),
    );
  }

  ngOnDestroy(): void {
    this._sub?.unsubscribe();
  }

  private computeStrength(pw: string): number {
    let score = 0;
    if (pw.length >= 8) score++;
    if (/[A-Z]/.test(pw)) score++;
    if (/[0-9]/.test(pw)) score++;
    if (/[^A-Za-z0-9]/.test(pw)) score++;
    return score;
  }
}
