import { Component, computed, input } from '@angular/core';

type MaxWidth = 'sm' | 'md' | 'lg' | 'xl';
type Padding = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-card',
  standalone: true,
  templateUrl: './card.html',
  styleUrl: './card.css',
})
export class Card {
  readonly maxWidth = input<MaxWidth>('md');
  readonly padding = input<Padding>('md');
  readonly centered = input(true);

  protected readonly containerClass = computed(() => {
    const w: Record<MaxWidth, string> = {
      sm: 'max-w-sm',
      md: 'max-w-md',
      lg: 'max-w-lg',
      xl: 'max-w-xl',
    };
    return `w-full ${w[this.maxWidth()]} ${this.centered() ? 'mx-auto' : ''}`.trim();
  });

  protected readonly cardClass = computed(() => {
    const p: Record<Padding, string> = { sm: 'p-5', md: 'p-8', lg: 'p-10' };
    return `bg-white dark:bg-slate-800 rounded-2xl shadow-sm border border-neutral-100 dark:border-slate-700 ${p[this.padding()]}`;
  });
}
