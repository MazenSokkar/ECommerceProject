
import { Routes } from '@angular/router';

export const SHIPPING_ROUTES: Routes = [
  {
    path: ':orderId',
    loadComponent: () =>
      import('./components/shipping-tracking/shipping-tracking').then(m => m.ShippingTracking),
  },
];