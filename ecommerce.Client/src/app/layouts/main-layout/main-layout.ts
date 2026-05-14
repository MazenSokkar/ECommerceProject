// src/app/layouts/main-layout/main-layout.ts

import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AuthApiService } from '../../features/auth/services/auth-api.service';
import { ThemeToggle } from '../../shared/components/theme-toggle/theme-toggle';
import { ROLES } from '../../shared/models/roles.model';
import { CartApiService } from '../../features/cart/services/cart-api-service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, ThemeToggle],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout implements OnInit {
  protected readonly auth = inject(AuthService);
  private readonly authApi = inject(AuthApiService);
  private readonly cartApi = inject(CartApiService);
  private readonly router = inject(Router);

  protected readonly profile = this.auth.profile;
  protected readonly displayName = computed(() => {
    const p = this.profile();
    return p ? `${p.firstName} ${p.lastName}` : 'User';
  });
  protected readonly initials = computed(() => {
    const p = this.profile();
    return p ? `${p.firstName[0]}${p.lastName[0]}`.toUpperCase() : 'U';
  });

  protected readonly isCustomer = computed(() => this.auth.hasRole(ROLES.Customer));
  protected readonly isAdmin = computed(() => this.auth.hasRole(ROLES.Admin));
  protected readonly isMerchant = computed(() => this.auth.hasRole(ROLES.Merchant));
  protected readonly cartCount = signal(0);
  protected readonly userMenuOpen = signal(false);
  protected readonly loggingOut = signal(false);

  ngOnInit(): void {
    if (this.isCustomer()) {
      this.loadCartCount();
    }
  }

  private loadCartCount(): void {
    this.cartApi.getCart().subscribe({
      next: (res) => this.cartCount.set(res.data?.totalItems ?? 0),
      error: () => this.cartCount.set(0),
    });
  }

  protected toggleUserMenu(): void {
    this.userMenuOpen.update((v) => !v);
  }

  protected async onLogout(): Promise<void> {
    this.loggingOut.set(true);
    this.authApi.logout().subscribe({
      complete: () => {
        this.auth.clearSession();
        this.router.navigate(['/login']);
      },
      error: () => {
        this.auth.clearSession();
        this.router.navigate(['/login']);
      },
    });
  }
}
