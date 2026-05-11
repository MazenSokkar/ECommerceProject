import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastItem {
  id: number;
  type: ToastType;
  title: string;
  message: string;
  exiting: boolean;
}

let nextId = 0;
const DISMISS_MS = 4000;
const EXIT_ANIMATION_MS = 300;

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toasts = signal<ToastItem[]>([]);

  success(title: string, message: string): void {
    this.add('success', title, message);
  }

  error(title: string, message: string): void {
    this.add('error', title, message);
  }

  warning(title: string, message: string): void {
    this.add('warning', title, message);
  }

  info(title: string, message: string): void {
    this.add('info', title, message);
  }

  dismiss(id: number): void {
    this.toasts.update((list) => list.map((t) => (t.id === id ? { ...t, exiting: true } : t)));
    setTimeout(() => {
      this.toasts.update((list) => list.filter((t) => t.id !== id));
    }, EXIT_ANIMATION_MS);
  }

  private add(type: ToastType, title: string, message: string): void {
    const id = nextId++;
    this.toasts.update((list) => [...list, { id, type, title, message, exiting: false }]);
    setTimeout(() => this.dismiss(id), DISMISS_MS);
  }
}
