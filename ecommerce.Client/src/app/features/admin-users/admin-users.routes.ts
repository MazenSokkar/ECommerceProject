import { Routes } from '@angular/router';

export const ADMIN_USERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/admin-users-page/admin-users-page').then((m) => m.AdminUsersPage),
  },
];
