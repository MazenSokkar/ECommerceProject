import { Component, computed, input } from '@angular/core';

type Variant = 'primary' | 'secondary' | 'outline' | 'danger' | 'ghost';
type Size = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-button',
  standalone: true,
  templateUrl: './button.html',
  styleUrl: './button.css',
})
export class Button {
  readonly variant = input<Variant>('primary');
  readonly size = input<Size>('md');
  readonly loading = input(false);
  readonly disabled = input(false);
  readonly fullWidth = input(false);
  readonly type = input<'button' | 'submit' | 'reset'>('button');
  readonly loadingText = input('Loading…');

  protected readonly isDisabled = computed(() => this.loading() || this.disabled());

  protected readonly classes = computed(() => {
    const width = this.fullWidth() ? 'w-full' : '';

    const sizes: Record<Size, string> = {
      sm: '!py-1.5 !px-3 !text-xs',
      md: '',
      lg: '!py-3.5 !px-7 !text-base',
    };

    const variants: Record<Variant, string> = {
      primary: 'btn-primary',
      danger: 'btn-danger',
      secondary: [
        'inline-flex items-center justify-center gap-2',
        'bg-secondary text-white font-bold text-sm rounded-xl',
        'border-none cursor-pointer py-2.5 px-5',
        'transition-opacity hover:opacity-90',
        'disabled:opacity-50 disabled:cursor-not-allowed',
      ].join(' '),
      outline: [
        'inline-flex items-center justify-center gap-2',
        'border border-neutral-200 dark:border-slate-600',
        'text-neutral-600 dark:text-slate-300 font-bold text-sm rounded-xl',
        'bg-transparent cursor-pointer py-2.5 px-5',
        'hover:bg-neutral-50 dark:hover:bg-slate-700 transition-colors',
        'disabled:opacity-50 disabled:cursor-not-allowed',
      ].join(' '),
      ghost: [
        'inline-flex items-center justify-center gap-2',
        'text-neutral-500 dark:text-slate-400 font-semibold text-sm rounded-xl',
        'bg-transparent border-none cursor-pointer py-2 px-3',
        'hover:text-neutral-700 dark:hover:text-slate-200 transition-colors',
      ].join(' '),
    };

    return [variants[this.variant()], sizes[this.size()], width].filter(Boolean).join(' ');
  });
}
