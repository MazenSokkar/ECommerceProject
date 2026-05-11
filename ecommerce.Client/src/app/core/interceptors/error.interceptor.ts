import { HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

function extractErrorMessage(err: {
  error?: {
    title?: string;
    detail?: string;
    errors?: string[];
    extensions?: { errors?: string[] };
  };
}): string | null {
  const body = err.error;
  if (!body) return null;

  const listErrors: string[] = body.errors ?? [];
  if (listErrors.length > 0) return listErrors.join(' ');

  const extensionErrors: string[] = body.extensions?.errors ?? [];
  if (extensionErrors.length > 0) return extensionErrors.join(' ');

  return body.detail ?? body.title ?? null;
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((err) => {
      const status: number = err.status;

      if (status === HttpStatusCode.Unauthorized) {
        auth.logout();
        return throwError(() => err);
      }

      if (status >= 500) {
        toast.error('Error', 'An unexpected error occurred. Please try again.');
        return throwError(() => err);
      }

      // 4xx — extract ProblemDetails message and surface it as a toast,
      // then re-throw so individual components can still handle it.
      if (status >= 400) {
        const message = extractErrorMessage(err);
        if (message) toast.error('Error', message);
      }

      return throwError(() => err);
    }),
  );
};
