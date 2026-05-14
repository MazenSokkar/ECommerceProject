import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LocationApiService } from '../../../../core/services/location-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { AuthService } from '../../../../core/services/auth.service';
import {
  CityResponse,
  CountryResponse,
  StateProvinceResponse,
} from '../../../../shared/models/location.model';
import { FormField } from '../../../../shared/components/form-field/form-field';
import { Button } from '../../../../shared/components/button/button';
import { ProfileApiService } from '../../services/profile-api.service';
import { UpdateUserProfileRequest, UserProfileResponse } from '../../models/profile.model';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormField, Button],
  templateUrl: './profile-page.html',
  styleUrl: './profile-page.css',
})
export class ProfilePage implements OnInit {
  private readonly api = inject(ProfileApiService);
  private readonly locationApi = inject(LocationApiService);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly profile = signal<UserProfileResponse | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly deleting = signal(false);
  protected readonly confirmingDelete = signal(false);

  protected readonly countries = signal<CountryResponse[]>([]);
  protected readonly states = signal<StateProvinceResponse[]>([]);
  protected readonly cities = signal<CityResponse[]>([]);
  protected readonly loadingStates = signal(false);
  protected readonly loadingCities = signal(false);

  protected readonly form = new FormGroup({
    firstName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    lastName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    phone: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/^(10|11|12|15)\d{8}$/)],
    }),
    locationName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    countryId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    stateProvinceId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    cityId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    latitude: new FormControl<string>('', { nonNullable: true }),
    longitude: new FormControl<string>('', { nonNullable: true }),
  });

  protected readonly isActive = computed(() => this.profile()?.active ?? false);

  ngOnInit(): void {
    this.locationApi.getCountries().subscribe({
      next: (res) => this.countries.set(res.data ?? []),
    });

    this.loadProfile();
  }

  protected onCountryChange(event: Event): void {
    const id = Number((event.target as HTMLSelectElement).value);
    this.form.patchValue({ stateProvinceId: null, cityId: null });
    this.states.set([]);
    this.cities.set([]);
    if (!id) return;
    this.loadingStates.set(true);
    this.locationApi.getStateProvincesByCountry(id).subscribe({
      next: (res) => {
        this.states.set(res.data ?? []);
        this.loadingStates.set(false);
      },
      error: () => this.loadingStates.set(false),
    });
  }

  protected onStateChange(event: Event): void {
    const id = Number((event.target as HTMLSelectElement).value);
    this.form.patchValue({ cityId: null });
    this.cities.set([]);
    if (!id) return;
    this.loadingCities.set(true);
    this.locationApi.getCitiesByStateProvince(id).subscribe({
      next: (res) => {
        this.cities.set(res.data ?? []);
        this.loadingCities.set(false);
      },
      error: () => this.loadingCities.set(false),
    });
  }

  protected onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.saving()) return;

    const v = this.form.getRawValue();
    const body: UpdateUserProfileRequest = {
      firstName: v.firstName,
      lastName: v.lastName,
      email: v.email,
      phone: this.normalizePhone(v.phone),
      address: {
        locationName: v.locationName,
        countryId: v.countryId!,
        stateProvinceId: v.stateProvinceId!,
        cityId: v.cityId!,
        latitude: v.latitude || null,
        longitude: v.longitude || null,
      },
    };

    this.saving.set(true);
    this.api.updateProfile(body).subscribe({
      next: (res) => {
        this.saving.set(false);
        if (res.data) {
          this.profile.set(res.data);
          this.toast.success('Success', 'Profile updated successfully.');
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
        if (res.data) this.patchForm(res.data);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  private patchForm(profile: UserProfileResponse): void {
    const address = profile.addresses[0];
    const phone = this.stripPhone(profile.phone ?? '');

    this.form.patchValue({
      firstName: profile.firstName,
      lastName: profile.lastName,
      email: profile.email,
      phone,
      locationName: address?.locationName ?? '',
      countryId: address?.countryId ?? null,
      stateProvinceId: address?.stateProvinceId ?? null,
      cityId: address?.cityId ?? null,
      latitude: address?.latitude ?? '',
      longitude: address?.longitude ?? '',
    });

    if (address?.countryId) {
      this.loadingStates.set(true);
      this.locationApi.getStateProvincesByCountry(address.countryId).subscribe({
        next: (res) => {
          this.states.set(res.data ?? []);
          this.loadingStates.set(false);
        },
        error: () => this.loadingStates.set(false),
      });
    }

    if (address?.stateProvinceId) {
      this.loadingCities.set(true);
      this.locationApi.getCitiesByStateProvince(address.stateProvinceId).subscribe({
        next: (res) => {
          this.cities.set(res.data ?? []);
          this.loadingCities.set(false);
        },
        error: () => this.loadingCities.set(false),
      });
    }
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
