import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { LocationApiService } from '../../../../core/services/location-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Button } from '../../../../shared/components/button/button';
import {
  UserFormModal,
  UserFormValue,
} from '../../../../shared/components/user-form-modal/user-form-modal';
import { ProfileApiService } from '../../services/profile-api.service';
import { UpdateUserProfileRequest, UserProfileResponse } from '../../models/profile.model';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, Button, UserFormModal],
  templateUrl: './profile-page.html',
  styleUrl: './profile-page.css',
})
export class ProfilePage implements OnInit {
  private readonly api = inject(ProfileApiService);
  private readonly locationApi = inject(LocationApiService);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  @ViewChild(UserFormModal) private modalRef!: UserFormModal;

  protected readonly profile = signal<UserProfileResponse | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly deleting = signal(false);
  protected readonly confirmingDelete = signal(false);
  protected readonly modalOpen = signal(false);

  protected readonly isActive = computed(() => this.profile()?.active ?? false);

  ngOnInit(): void {
    this.loadProfile();
  }

  protected openEditModal(): void {
    const p = this.profile();
    if (!p) return;
    this.modalOpen.set(true);

    // After modal opens, patch the form with profile data
    setTimeout(() => {
      if (!this.modalRef) return;
      const address = p.addresses[0];
      this.modalRef.patchAddress({
        firstName: p.firstName,
        lastName: p.lastName,
        email: p.email,
        phone: this.stripPhone(p.phone ?? ''),
        locationName: address?.locationName ?? '',
        countryId: address?.countryId ?? null,
        stateProvinceId: address?.stateProvinceId ?? null,
        cityId: address?.cityId ?? null,
        latitude: address?.latitude ?? '',
        longitude: address?.longitude ?? '',
      });
    });
  }

  protected closeModal(): void {
    this.modalOpen.set(false);
  }

  protected onFormSubmitted(value: UserFormValue): void {
    if (this.saving()) return;

    const body: UpdateUserProfileRequest = {
      firstName: value.firstName,
      lastName: value.lastName,
      email: value.email,
      phone: this.normalizePhone(value.phone),
      address: {
        locationName: value.locationName,
        countryId: value.countryId!,
        stateProvinceId: value.stateProvinceId!,
        cityId: value.cityId!,
        latitude: value.latitude || null,
        longitude: value.longitude || null,
      },
    };

    this.saving.set(true);
    this.api.updateProfile(body).subscribe({
      next: (res) => {
        this.saving.set(false);
        if (res.data) {
          this.profile.set(res.data);
          this.toast.success('Success', 'Profile updated successfully.');
          this.closeModal();
        }
      },
      error: () => {
        this.saving.set(false);
        this.toast.error('Error', 'Failed to update profile.');
      },
    });
  }

  protected startDelete(): void {
    this.confirmingDelete.set(true);
  }

  protected cancelDelete(): void {
    this.confirmingDelete.set(false);
  }

  protected deleteAccount(): void {
    if (this.deleting()) return;
    this.deleting.set(true);
    this.api.deleteProfile().subscribe({
      next: () => {
        this.deleting.set(false);
        this.toast.success('Success', 'Account deleted successfully.');
        this.auth.clearSession();
        this.router.navigate(['/login']);
      },
      error: () => {
        this.deleting.set(false);
        this.toast.error('Error', 'Failed to delete account.');
      },
    });
  }

  private loadProfile(): void {
    this.loading.set(true);
    this.api.getProfile().subscribe({
      next: (res) => {
        this.profile.set(res.data ?? null);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  private stripPhone(value: string): string {
    if (!value) return '';
    return value.startsWith('+20') ? value.slice(3) : value;
  }

  private normalizePhone(value: string): string {
    if (!value) return value;
    return value.startsWith('+20') ? value : `+20${value}`;
  }
}
