// src/app/features/orders/orders.routes.ts

import { Routes } from '@angular/router';

export const ORDERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/order-list/order-list').then(m => m.OrderList),
  },
  {
    path: 'checkout',
    loadComponent: () =>
      import('./components/checkout-page/checkout-page').then(m => m.CheckoutPage),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./components/order-detail/order-detail').then(m => m.OrderDetailComponent),
  },
];