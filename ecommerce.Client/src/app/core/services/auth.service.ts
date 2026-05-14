import { Injectable, PLATFORM_ID, computed, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { DecodedUser } from '../../shared/models/decoded-user.model';
import { UserProfile } from '../../features/auth/models/auth.model';

const TOKEN_KEY = 'gw_token';
const PROFILE_KEY = 'gw_profile';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly router = inject(Router);

  private readonly _token = signal<string | null>(this.readStorage(TOKEN_KEY));
  private readonly _decoded = signal<DecodedUser | null>(this.decodeToken(this.readStorage(TOKEN_KEY)));
  private readonly _profile = signal<UserProfile | null>(this.readProfile());

  readonly token = this._token.asReadonly();
  readonly profile = this._profile.asReadonly();
  readonly isAuthenticated = computed(() => {
    const decoded = this._decoded();
    if (!decoded) return false;
    if (decoded.exp && Date.now() / 1000 > decoded.exp) return false;
    return true;
  });
  readonly roles = computed(() => this._decoded()?.roles ?? []);

  setSession(token: string, profile: UserProfile): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(TOKEN_KEY, token);
      localStorage.setItem(PROFILE_KEY, JSON.stringify(profile));
    }
    this._token.set(token);
    this._decoded.set(this.decodeToken(token));
    this._profile.set(profile);
  }

  clearSession(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(PROFILE_KEY);
    }
    this._token.set(null);
    this._decoded.set(null);
    this._profile.set(null);
  }

  logout(): void {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  hasRole(role: string): boolean {
    return this.roles().includes(role);
  }

  private readStorage(key: string): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(key);
  }

  private readProfile(): UserProfile | null {
    const raw = this.readStorage(PROFILE_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as UserProfile;
    } catch {
      return null;
    }
  }

  private decodeToken(token: string | null): DecodedUser | null {
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const raw = payload['role'] ?? payload['roles'];
      return {
        sub: payload['sub'] ?? payload['nameid'] ?? '',
        email: payload['email'] ?? '',
        roles: Array.isArray(raw) ? raw : raw ? [raw] : [],
        exp: payload['exp'],
      };
    } catch {
      return null;
    }
  }

  getUserId(): number | null {
  const decoded = this._decoded();
  if (!decoded?.sub) return null;

  return Number(decoded.sub);
}

}
