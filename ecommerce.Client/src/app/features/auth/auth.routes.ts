import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./components/login/login').then(m => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./components/register/register').then(m => m.Register),
  },
  {
    path: 'confirm-email',
    loadComponent: () => import('./components/confirm-email/confirm-email').then(m => m.ConfirmEmail),
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./components/forgot-password/forgot-password').then(m => m.ForgotPassword),
  },
];
