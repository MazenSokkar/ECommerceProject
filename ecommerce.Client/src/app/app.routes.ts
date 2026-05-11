import { Routes } from '@angular/router';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { MainLayout } from './layouts/main-layout/main-layout';
import { authGuard } from './core/guards/auth.guard';
import { AUTH_ROUTES } from './features/auth/auth.routes';

export const routes: Routes = [
  // Landing page
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/landing/components/landing/landing').then(m => m.Landing),
  },

  // Auth screens (split-panel layout)
  {
    path: '',
    component: AuthLayout,
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      ...AUTH_ROUTES,
    ],
  },

  // Protected app shell
  {
    path: 'app',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      // Sprint 2+ feature routes go here
    ],
  },

  { path: '**', redirectTo: '' },
];
