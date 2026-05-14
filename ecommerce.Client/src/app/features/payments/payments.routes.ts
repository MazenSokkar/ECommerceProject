
import { Routes } from '@angular/router';

export const PAYMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/payment-page/payment-page').then(m => m.PaymentPage),
  },
];