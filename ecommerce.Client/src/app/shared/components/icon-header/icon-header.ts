import { Component, computed, input } from '@angular/core';

type IconSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-icon-header',
  standalone: true,
  templateUrl: './icon-header.html',
  styleUrl: './icon-header.css',
})
export class IconHeader {
  readonly icon = input('');
  readonly gradient = input('linear-gradient(135deg, #0E7C7B, #14B8A6)');
  readonly size = input<IconSize>('md');

  protected readonly containerClass = computed(() => {
    const sizes: Record<IconSize, string> = {
      sm: 'w-12 h-12',
      md: 'w-16 h-16',
      lg: 'w-20 h-20',
    };
    return `rounded-2xl flex items-center justify-center ${sizes[this.size()]}`;
  });

  protected readonly iconClass = computed(() => {
    const sizes: Record<IconSize, string> = {
      sm: 'text-2xl',
      md: 'text-3xl',
      lg: 'text-4xl',
    };
    return `${this.icon()} text-white ${sizes[this.size()]}`;
  });
}
