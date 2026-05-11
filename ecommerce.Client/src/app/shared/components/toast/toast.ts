import { Component, inject } from '@angular/core';
import { ToastService, ToastType } from '../../../core/services/toast.service';

interface ToastTheme {
  waveColor: string;
  iconBg: string;
  iconColor: string;
  textColor: string;
}

const THEMES: Record<ToastType, ToastTheme> = {
  success: {
    waveColor: '#10b9813a',
    iconBg: '#10b98148',
    iconColor: '#059669',
    textColor: '#059669',
  },
  error: {
    waveColor: '#fc0c0c3a',
    iconBg: '#fc0c0c48',
    iconColor: '#d10d0d',
    textColor: '#d10d0d',
  },
  warning: {
    waveColor: '#ffa30d3a',
    iconBg: '#ffa30d48',
    iconColor: '#db970e',
    textColor: '#db970e',
  },
  info: { waveColor: '#4777ff3a', iconBg: '#4777ff48', iconColor: '#124fff', textColor: '#124fff' },
};

@Component({
  selector: 'app-toast',
  imports: [],
  templateUrl: './toast.html',
  styleUrl: './toast.css',
})
export class Toast {
  protected readonly svc = inject(ToastService);

  protected theme(type: ToastType): ToastTheme {
    return THEMES[type];
  }

  protected cssVars(type: ToastType): Record<string, string> {
    const t = THEMES[type];
    return {
      '--wave-color': t.waveColor,
      '--icon-bg': t.iconBg,
      '--icon-color': t.iconColor,
      '--text-color': t.textColor,
    };
  }

  protected dismiss(id: number): void {
    this.svc.dismiss(id);
  }
}
