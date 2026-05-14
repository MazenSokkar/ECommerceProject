import { Routes } from '@angular/router';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { MainLayout } from './layouts/main-layout/main-layout';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { AUTH_ROUTES } from './features/auth/auth.routes';
import { ROLES } from './shared/models/roles.model';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/landing/components/landing/landing').then((m) => m.Landing),
  },

  {
    path: '',
    component: AuthLayout,
    children: [{ path: '', redirectTo: 'login', pathMatch: 'full' }, ...AUTH_ROUTES],
  },

  {
    path: 'app',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadChildren: () =>
          import('./features/home/home.routes').then((m) => m.HOME_ROUTES),
      },
      {
        path: 'products',
        loadChildren: () =>
          import('./features/products/products.routes').then((m) => m.PRODUCTS_ROUTES),
      },
      {
        path: 'wishlist',
        loadChildren: () =>
          import('./features/wishlist/wishlist.routes').then((m) => m.WISHLIST_ROUTES),
      },
      {
        path: 'profile',
        loadChildren: () =>
          import('./features/profile/profile.routes').then((m) => m.PROFILE_ROUTES),
      },
      {
        path: 'merchant',
        loadChildren: () =>
          import('./features/merchant/merchant.routes').then((m) => m.MERCHANT_ROUTES),
      },
      {
        path: 'admin',
        canActivate: [roleGuard(ROLES.Admin)],
        loadChildren: () =>
          import('./features/admin-dashboard/admin-dashboard.routes').then((m) => m.ADMIN_DASHBOARD_ROUTES),
      },
      {
        path: 'cart',
        loadChildren: () => import('./features/cart/Cart.routes').then((c) => c.CART_ROUTES),
      },
      {
        path: 'orders',
        loadChildren: () => import('./features/orders/orders.routes').then((m) => m.ORDERS_ROUTES),
      },
      {
        path: 'payment',
        loadChildren: () =>
          import('./features/payments/payments.routes').then((m) => m.PAYMENTS_ROUTES),
      },
      {
        path: 'shipping',
        loadChildren: () =>
          import('./features/shipping/shipping.routes').then((m) => m.SHIPPING_ROUTES),
      },
    ],
  },

  { path: '**', redirectTo: '' },
];
