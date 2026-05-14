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

  private readonly _token = signal<string | null>(this.readCookie(TOKEN_KEY));
  private readonly _decoded = signal<DecodedUser | null>(this.decodeToken(this.readCookie(TOKEN_KEY)));
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
    this.writeCookie(TOKEN_KEY, token);
    this.writeCookie(PROFILE_KEY, JSON.stringify(profile));
    
    this._token.set(token);
    this._decoded.set(this.decodeToken(token));
    this._profile.set(profile);
  }

  clearSession(): void {
    this.deleteCookie(TOKEN_KEY);
    this.deleteCookie(PROFILE_KEY);
    
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

  private readCookie(key: string): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    const nameEQ = key + '=';
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') c = c.substring(1, c.length);
      if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
  }

  private writeCookie(key: string, value: string, days = 7): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const date = new Date();
    date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
    const expires = `; expires=${date.toUTCString()}`;
    document.cookie = `${key}=${value || ''}${expires}; path=/; SameSite=Lax`;
  }

  private deleteCookie(key: string): void {
    if (!isPlatformBrowser(this.platformId)) return;
    document.cookie = `${key}=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT; SameSite=Lax`;
  }

  private readProfile(): UserProfile | null {
    const raw = this.readCookie(PROFILE_KEY);
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
