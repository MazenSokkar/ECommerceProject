import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AuthApiService } from '../../features/auth/services/auth-api.service';
import { ThemeToggle } from '../../shared/components/theme-toggle/theme-toggle';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, ThemeToggle],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {
  protected readonly auth = inject(AuthService);
  private readonly authApi = inject(AuthApiService);
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

  protected readonly userMenuOpen = signal(false);
  protected readonly loggingOut = signal(false);

  protected toggleUserMenu(): void {
    this.userMenuOpen.update(v => !v);
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
