# GiftWave Frontend — Architecture & Implementation Guide

This file is the single source of truth for every frontend pattern. Follow it exactly — do not invent patterns not described here.

---

## Tech Stack

- **Angular 21** — standalone components only, no NgModules
- **TypeScript 5.9** — strict mode enabled (`strict: true`, `strictTemplates: true`)
- **RxJS 7.8** — HTTP calls and complex async streams only
- **Angular Signals** — preferred state management pattern everywhere else
- **Tailwind CSS 4** — utility-first styling, no inline `style=""` attributes
- **Angular SSR + Express** — server-side rendering enabled
- **Prettier** — enforced via `.prettierrc` in `package.json` (printWidth 100, singleQuote)

---

## Folder Structure

```
src/app/
├── core/               # App-wide singletons — loaded once
│   ├── services/       # auth.service, toast.service, theme.service
│   ├── guards/         # auth.guard, role.guard
│   └── interceptors/   # auth.interceptor, error.interceptor
│
├── features/           # One folder per domain — all lazy-loaded
│   └── {feature}/
│       ├── {feature}.routes.ts
│       ├── services/
│       │   └── {feature}-api.service.ts
│       ├── models/
│       │   └── {entity}.model.ts
│       └── components/
│           └── {component}/
│               ├── {component}.ts
│               ├── {component}.html
│               └── {component}.css
│
├── layouts/
│   ├── main-layout/    # Authenticated shell: Header + Sidebar + <router-outlet>
│   └── auth-layout/    # Public shell: clean centered card, no nav
│
└── shared/
    ├── components/     # Dumb/presentational reusable UI (spinner, otp-input, etc.)
    ├── directives/     # e.g., click-outside
    ├── pipes/          # e.g., date-format, truncate
    └── models/
        ├── api-response.model.ts
        ├── pagination.model.ts
        └── roles.model.ts
```

---

## Dependency Rules

```
features/*  →  may inject from core/ and shared/
core/*      →  must NEVER import from features/*
shared/*    →  must NEVER import from features/ or core/services/
layouts/*   →  may inject from core/ only
```

---

## Adding a New Feature — Checklist

### 1. Define models first (`models/{entity}.model.ts`)

```typescript
export interface GiftCard {
  id: string;
  title: string;
  value: number;
}
```

### 2. Create the API service (`services/{feature}-api.service.ts`)

```typescript
@Injectable({ providedIn: 'root' })
export class GiftCardsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/gift-cards`;

  getAll(): Observable<ApiResponse<GiftCard[]>> {
    return this.http.get<ApiResponse<GiftCard[]>>(this.baseUrl);
  }
}
```

### 3. Create the component with signal-based state

```typescript
@Component({ selector: 'app-gift-card-list', standalone: true, imports: [...] })
export class GiftCardList {
  private readonly api = inject(GiftCardsApiService);

  protected readonly items = signal<GiftCard[]>([]);
  protected readonly isLoading = signal(false);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.isLoading.set(true);
    this.api.getAll().subscribe({
      next: res => { this.items.set(res.data); this.isLoading.set(false); },
      error: err => { this.error.set(err.message); this.isLoading.set(false); },
    });
  }
}
```

### 4. Define feature routes (`{feature}.routes.ts`)

```typescript
export const GIFT_CARDS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/gift-card-list/gift-card-list').then(m => m.GiftCardList),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./components/gift-card-detail/gift-card-detail').then(m => m.GiftCardDetail),
  },
];
```

### 5. Register under `MainLayout` in `app.routes.ts`

```typescript
{
  path: '',
  component: MainLayout,
  canActivate: [authGuard],
  children: [
    {
      path: 'gift-cards',
      loadChildren: () =>
        import('./features/gift-cards/gift-cards.routes').then(m => m.GIFT_CARDS_ROUTES),
    },
  ],
}
```

### 6. Add role-based UI visibility (required for role-gated actions)

**In the component (`.ts`):**
```typescript
private readonly auth = inject(AuthService);

protected readonly isAdmin = computed(() => this.auth.hasRole(ROLES.Admin));
protected readonly isMerchant = computed(() => this.auth.hasRole(ROLES.Merchant));

protected delete(id: string): void {
  if (!this.isAdmin()) return;
  // proceed
}
```

**In the template (`.html`):**
```html
@if (isAdmin()) {
  <button (click)="delete(item.id)">Delete</button>
}
```

---

## Routing Conventions

- All `loadComponent` calls use `.then(m => m.ClassName)` — named exports only
- All `loadChildren` calls use `.then(m => m.FEATURE_ROUTES)`
- Public routes (login, register, forgot-password) — inside `AuthLayout`
- Protected routes (all app pages) — inside `MainLayout` with `canActivate: [authGuard]`
- Role-restricted routes — add `canActivate: [roleGuard(ROLES.Admin)]` after `authGuard`
- Wildcard `**` — `redirectTo: 'login'`

---

## State Management — Angular Signals

Use Signals for all component and service state. RxJS only for HTTP and streams you cannot express as signals.

```typescript
// core service example
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _currentUser = signal<DecodedUser | null>(null);
  readonly currentUser = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => this._currentUser() !== null);
  readonly roles = computed(() => this._currentUser()?.roles ?? []);

  hasRole(role: string): boolean {
    return this.roles().includes(role);
  }
}
```

Use `computed()` for derived state. Use `effect()` sparingly — side effects only (e.g., persisting theme to `localStorage`).

---

## HTTP Layer

Configure everything in `app.config.ts`:

```typescript
export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    provideRouter(routes, withComponentInputBinding()),
    provideClientHydration(withEventReplay()),
  ],
};
```

- `authInterceptor` — attaches `Authorization: Bearer <token>` from the cookie to every outbound request
- `errorInterceptor` — handles 401 (call `auth.logout()` → redirect to `/login`), 500 (show toast), network errors

---

## Authentication & Role System

### JWT Cookie

Login sets a **readable JS cookie** named `token`. The `AuthService` reads it with `document.cookie`, decodes the payload (base64), and extracts claims. Guard all `document.cookie` access with `isPlatformBrowser(this.platformId)` for SSR safety.

```typescript
// Token keys
const TOKEN_COOKIE = 'token';

// Cookie read (browser-only)
private readTokenFromCookie(): string | null {
  if (!isPlatformBrowser(this.platformId)) return null;
  const match = document.cookie.match(/(?:^|; )token=([^;]*)/);
  return match ? decodeURIComponent(match[1]) : null;
}
```

### Decoded user shape

```typescript
// shared/models/decoded-user.model.ts
export interface DecodedUser {
  sub: string;           // user id
  email: string;
  roles: string[];       // from 'role' claim — handle single string or array
}
```

> The `role` claim in a JWT can be a single string or an array. Always normalize to `string[]`:
> ```typescript
> const raw = payload['role'];
> roles: Array.isArray(raw) ? raw : raw ? [raw] : []
> ```

### Roles constants

```typescript
// shared/models/roles.model.ts
export const ROLES = {
  Admin: 'Admin',
  Merchant: 'Merchant',
  Corporate: 'Corporate',
  Receiver: 'Receiver',
  Customer: 'Customer',
} as const;
```

Always import from `ROLES` — never hardcode role strings in templates or components.

### Role-based UI pattern summary

| Scenario | Pattern |
|---|---|
| Route only accessible to a specific role | `canActivate: [roleGuard(ROLES.Admin)]` in route definition |
| Component method requires a role | Add `if (!this.isAdmin()) return;` at method top |
| UI section/button hidden by role | `@if (isAdmin()) { ... }` in template |

### OTP flow (reuse for registration and forgot-password)

1. **Submit form** — API sends OTP to the user's email
2. **OTP input screen** — 6-digit input, auto-advance between digits
3. **Resend** — disabled for 60 seconds, countdown shown, re-enabled after
4. **Verify** — on success, advance to next step; on failure, shake animation + clear + refocus
5. **Forgot-password wizard** — 3 steps: Enter email → Enter OTP → Set new password

---

## Shared Models

Always use these in API services:

```typescript
// shared/models/api-response.model.ts
export interface ApiResponse<T> {
  data: T;
  message: string;
  isSuccess: boolean;
}

// shared/models/pagination.model.ts
export interface Paginated<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
```

---

## SSR Render Modes (`app.routes.server.ts`)

```typescript
export const serverRoutes: ServerRoute[] = [
  { path: 'login', renderMode: RenderMode.Prerender },
  { path: 'register', renderMode: RenderMode.Prerender },
  { path: 'forgot-password', renderMode: RenderMode.Prerender },
  { path: '**', renderMode: RenderMode.Server }, // auth-required pages — never prerender
];
```

---

## Environment Variables

```typescript
// environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7xxx/api',
};
```

Access in services via direct import — not a token.

---

## Color Palette

### Brand Colors

| Role | Name | Hex |
|---|---|---|
| Primary | Royal Teal | `#0E7C7B` |
| Secondary | Warm Amber | `#F59E0B` |
| Accent | Coral Pink | `#EC4899` |
| Success | Verdant Green | `#059669` |
| Warning | Saffron | `#D97706` |
| Danger | Crimson | `#DC2626` |

### Neutral Scale

| Token | Hex | Use |
|---|---|---|
| neutral-50 | `#F9FAFB` | Page background (light mode) |
| neutral-100 | `#F3F4F6` | Card backgrounds, secondary surfaces |
| neutral-200 | `#E5E7EB` | Borders, dividers, disabled states |
| neutral-400 | `#9CA3AF` | Placeholder text, secondary icons |
| neutral-600 | `#4B5563` | Body text, secondary headings |
| neutral-800 | `#1F2937` | Primary text, primary headings |
| neutral-900 | `#111827` | High-contrast text |

### Dark Mode Palette

| Token | Light | Dark |
|---|---|---|
| Background | `#F9FAFB` | `#0F172A` |
| Surface | `#FFFFFF` | `#1E293B` |
| Primary text | `#1F2937` | `#F1F5F9` |
| Primary action | `#0E7C7B` | `#14B8A6` |
| Border | `#E5E7EB` | `#334155` |

**Never use violet, indigo, or purple.** Always use the teal brand palette for primary actions.

---

## UI Design System

### Styling Rules

- **Tailwind first** — utility classes for all layout, spacing, color.
- **No inline `style=""` attributes** — if a style can't be expressed cleanly in Tailwind, add a CSS class in the component's `.css` file.
- **All pages must be fully responsive** — mobile-first, use `sm:` / `lg:` breakpoints.

### Layout Pattern

- **No `max-w-*` containers** on feature pages — content fills the full available width.
- **Mobile:** single-column stacked layout.
- **Desktop (`lg:`):** multi-column grid:
  - List/table pages: full-width table with responsive card fallback for mobile.
  - Form pages: `grid grid-cols-1 lg:grid-cols-3` — form takes `lg:col-span-2`, summary card takes 1 col.
  - Detail pages: `grid grid-cols-1 lg:grid-cols-2`.
  - Dashboard pages: `grid grid-cols-1 lg:grid-cols-2` or `lg:grid-cols-3`.

### Responsive Table Pattern

Desktop table + mobile card list — always both:
```html
<!-- Desktop -->
<table class="hidden sm:table w-full">...</table>

<!-- Mobile -->
<ul class="sm:hidden divide-y">
  @for (item of items(); track item.id) {
    <li class="p-4">...</li>
  }
</ul>
```

### Shared Design Token Classes (`src/styles.css`)

Always use these — never reinvent them per component:

| Class | Purpose |
|---|---|
| `.page-header` | Gradient banner at the top of every feature page |
| `.btn-primary` | Main CTA button (teal gradient) |
| `.btn-accent` | Secondary button inside dark headers |
| `.btn-danger` | Destructive action button (red gradient) |
| `.section-card` | White rounded card with brand shadow |
| `.card-stripe` | 4px teal stripe on top of a card (use with `.section-card`) |
| `.data-card` | Flat card for table wrappers |
| `.table-head` | Gradient `<thead>` background |
| `.avatar-circle` | Brand-gradient circle for user initials |
| `.role-badge` | Teal pill badge for role names |
| `.status-active` | Green pill badge |
| `.form-input` | Styled text input with focus ring |
| `.form-input--error` | Error state for form inputs |
| `.empty-icon-box` | Gradient box for empty-state icons |

### Page Structure Template

```html
<div class="space-y-6">
  <!-- Page header -->
  <div class="page-header rounded-2xl p-5 sm:p-6 flex flex-col sm:flex-row sm:items-center gap-4 sm:justify-between">
    <div>
      <h1 class="text-2xl font-extrabold text-white tracking-tight">Page Title</h1>
      <p class="text-teal-200 text-sm mt-0.5">Subtitle</p>
    </div>
    <button class="btn-accent self-start sm:self-auto px-5 py-2.5 rounded-xl text-sm font-bold">
      Action
    </button>
  </div>

  <!-- Content -->
  <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
    <div class="section-card card-stripe">
      <div class="p-5 sm:p-6">
        <!-- content -->
      </div>
    </div>
  </div>
</div>
```

### Loading & Empty States

- **Loading:** standard SVG spinner with `animate-spin text-[#0E7C7B]` inside a centered `py-24` wrapper.
- **Empty:** centered `py-24` with `.empty-icon-box` icon + short message in `text-neutral-400`.

### Form Inputs

- Always apply `.form-input` on every `<input>` and `<select>`.
- Apply `.form-input--error` conditionally: `[class.form-input--error]="control.invalid && control.touched"`.
- Error messages in `<p class="text-[#DC2626] text-xs mt-1">` below each field.
- Wrap multi-section forms in a single `<form [formGroup]="form">` spanning the full grid.

---

## i18n & RTL

- Language choice persisted in `localStorage` under key `lang` (`"ar"` or `"en"`).
- `ThemeService` (or a dedicated `LocaleService`) applies `dir="rtl"` on `<html>` and switches the active `lang` attribute.
- All static text uses translation keys — never hardcode UI strings in templates.
- RTL-aware spacing: prefer `ms-*` / `me-*` (margin-start/end) over `ml-*` / `mr-*`.

---

## Dark Mode

- Toggle stored in `localStorage` under key `theme` (`"dark"` or `"light"`).
- `ThemeService` applies the `dark` class on `<html>` for Tailwind's dark mode variant.
- All color classes must include a `dark:` variant where contrast requires it.

---

## Toast Notifications

Inject `ToastService` from `core/services/toast.service.ts` anywhere in the app:

```typescript
private readonly toast = inject(ToastService);

this.toast.success('Gift card sent!');
this.toast.error('Something went wrong.');
this.toast.info('OTP resent to your email.');
```

Never use `alert()` or browser dialogs.

---

## What NOT to Do

- Do not create NgModules — standalone components only
- Do not put feature-specific logic in `core/`
- Do not import from `features/` inside `core/` or `shared/`
- Do not use `RenderMode.Prerender` for auth-protected routes
- Do not use `import('./feature')` in routes without `.then(m => m.ClassName)`
- Do not use NgRx — Signals handle all state
- Do not skip the `models/` step — define interfaces before writing services
- Do not use violet, indigo, or purple — teal brand palette only
- Do not use inline `style=""` attributes — CSS classes only
- Do not constrain feature pages with `max-w-*` — fill the full width
- Do not hardcode role strings — always use `ROLES` constants
- Do not show destructive actions without a role check — gate with `@if (isAdmin())`
- Do not call mutating API methods without checking the role — guard with `if (!isAdmin()) return;`
- Do not read `document.cookie` or `localStorage` without an `isPlatformBrowser` guard
