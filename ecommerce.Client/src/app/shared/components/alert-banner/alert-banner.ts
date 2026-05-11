import { Component, computed, input } from '@angular/core';

type AlertType = 'warning' | 'danger' | 'success' | 'info';

@Component({
  selector: 'app-alert-banner',
  standalone: true,
  templateUrl: './alert-banner.html',
  styleUrl: './alert-banner.css',
})
export class AlertBanner {
  readonly type = input<AlertType>('info');
  readonly message = input('');
  readonly icon = input('');

  protected readonly wrapperClass = computed(() => {
    const map: Record<AlertType, string> = {
      warning: 'bg-warning/10 border-warning/30 text-warning',
      danger:  'bg-danger/10 border-danger/30 text-danger',
      success: 'bg-success/10 border-success/30 text-success',
      info:    'bg-primary/10 border-primary/30 text-primary',
    };
    return `p-3 rounded-xl border text-sm ${map[this.type()]}`;
  });

  protected readonly resolvedIcon = computed(() => {
    if (this.icon()) return this.icon();
    const map: Record<AlertType, string> = {
      warning: 'lni lni-warning',
      danger:  'lni lni-close-circle',
      success: 'lni lni-checkmark-circle',
      info:    'lni lni-information',
    };
    return map[this.type()];
  });
}
