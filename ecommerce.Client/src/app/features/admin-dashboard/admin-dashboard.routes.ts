import { Routes } from '@angular/router';

export const ADMIN_DASHBOARD_ROUTES: Routes = [
  { path: '', redirectTo: 'overview', pathMatch: 'full' },
  {
    path: 'overview',
    loadComponent: () =>
      import('./components/admin-overview/admin-overview').then((m) => m.AdminOverview),
  },
  {
    path: 'users',
    loadComponent: () =>
      import('./components/admin-users-page/admin-users-page').then((m) => m.AdminUsersPage),
  },
  {
    path: 'banners',
    loadComponent: () =>
      import('./components/admin-banners-page/admin-banners-page').then(
        (m) => m.AdminBannersPage
      ),
  },
];
