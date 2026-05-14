import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocationApiService } from '../../../../core/services/location-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import {
  CityResponse,
  CountryResponse,
  StateProvinceResponse,
} from '../../../../shared/models/location.model';
import { FormField } from '../../../../shared/components/form-field/form-field';
import { Button } from '../../../../shared/components/button/button';
import { PasswordInput } from '../../../../shared/components/password-input/password-input';
import { AdminUsersApiService } from '../../services/admin-users-api.service';
import {
  AdminUser,
  AdminUsersByRoleResponse,
  CreateAdminUserRequest,
  UpdateAdminUserRequest,
} from '../../models/admin-users.model';

@Component({
  selector: 'app-admin-users-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormField, Button, PasswordInput],
  templateUrl: './admin-users-page.html',
  styleUrl: './admin-users-page.css',
})
export class AdminUsersPage implements OnInit {
  private readonly api = inject(AdminUsersApiService);
  private readonly locationApi = inject(LocationApiService);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly togglingId = signal<number | null>(null);
  protected readonly deletingId = signal<number | null>(null);
  protected readonly includeDeleted = signal(false);
  protected readonly groups = signal<AdminUsersByRoleResponse | null>(null);
  protected readonly activeTab = signal<'customers' | 'admins' | 'merchants'>('customers');
  protected readonly editingUser = signal<AdminUser | null>(null);

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
    password: new FormControl('', { nonNullable: true }),
    locationName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    countryId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    stateProvinceId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    cityId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    latitude: new FormControl<string>('', { nonNullable: true }),
    longitude: new FormControl<string>('', { nonNullable: true }),
  });

  protected readonly isEditing = computed(() => this.editingUser() !== null);

  protected readonly currentUsers = computed(() => {
    const data = this.groups();
    if (!data) return [] as AdminUser[];
    if (this.activeTab() === 'admins') return data.admins;
    if (this.activeTab() === 'merchants') return data.merchants;
    return data.customers;
  });

  ngOnInit(): void {
    this.locationApi.getCountries().subscribe({
      next: (res) => this.countries.set(res.data ?? []),
    });

    this.setPasswordValidators(true);
    this.loadUsers();
  }

  protected changeTab(tab: 'customers' | 'admins' | 'merchants'): void {
    this.activeTab.set(tab);
  }

  protected toggleIncludeDeleted(): void {
    this.includeDeleted.update((v) => !v);
    this.loadUsers();
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

  protected openCreate(): void {
    this.editingUser.set(null);
    this.form.reset();
    this.states.set([]);
    this.cities.set([]);
    this.setPasswordValidators(true);
  }

  protected openEdit(user: AdminUser): void {
    this.editingUser.set(user);
    this.setPasswordValidators(false);
    this.form.patchValue({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phone: this.stripPhone(user.phone),
      password: '',
      locationName: '',
      countryId: null,
      stateProvinceId: null,
      cityId: null,
      latitude: '',
      longitude: '',
    });

    this.locationApi.getAddressesByUser(user.id).subscribe({
      next: (res) => {
        const address = res.data?.[0];
        if (!address) return;
        this.form.patchValue({
          locationName: address.locationName,
          countryId: address.countryId,
          stateProvinceId: address.stateProvinceId,
          cityId: address.cityId,
          latitude: address.latitude ?? '',
          longitude: address.longitude ?? '',
        });

        this.loadingStates.set(true);
        this.locationApi.getStateProvincesByCountry(address.countryId).subscribe({
          next: (statesRes) => {
            this.states.set(statesRes.data ?? []);
            this.loadingStates.set(false);
          },
          error: () => this.loadingStates.set(false),
        });

        this.loadingCities.set(true);
        this.locationApi.getCitiesByStateProvince(address.stateProvinceId).subscribe({
          next: (citiesRes) => {
            this.cities.set(citiesRes.data ?? []);
            this.loadingCities.set(false);
          },
          error: () => this.loadingCities.set(false),
        });
      },
    });
  }

  protected cancelEdit(): void {
    this.editingUser.set(null);
    this.form.reset();
    this.states.set([]);
    this.cities.set([]);
    this.setPasswordValidators(true);
  }

  protected onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.saving()) return;

    const v = this.form.getRawValue();
    const address = {
      locationName: v.locationName,
      countryId: v.countryId!,
      stateProvinceId: v.stateProvinceId!,
      cityId: v.cityId!,
      latitude: v.latitude || null,
      longitude: v.longitude || null,
    };

    this.saving.set(true);

    if (this.isEditing()) {
      const user = this.editingUser()!;
      const body: UpdateAdminUserRequest = {
        firstName: v.firstName,
        lastName: v.lastName,
        email: v.email,
        phone: this.normalizePhone(v.phone),
        address,
      };
      this.api.update(user.id, body).subscribe({
        next: () => {
          this.saving.set(false);
          this.toast.success('Success', 'User updated successfully.');
          this.loadUsers();
          this.cancelEdit();
        },
        error: () => {
          this.saving.set(false);
          this.toast.error('Error', 'Failed to update user.');
        },
      });
      return;
    }

    const body: CreateAdminUserRequest = {
      firstName: v.firstName,
      lastName: v.lastName,
      email: v.email,
      phone: this.normalizePhone(v.phone),
      password: v.password,
      address,
    };

    this.api.create(body).subscribe({
      next: () => {
        this.saving.set(false);
        this.toast.success('Success', 'User created successfully.');
        this.loadUsers();
        this.form.reset();
        this.setPasswordValidators(true);
      },
      error: () => {
        this.saving.set(false);
        this.toast.error('Error', 'Failed to create user.');
      },
    });
  }

  protected toggleStatus(user: AdminUser): void {
    if (this.togglingId()) return;
    this.togglingId.set(user.id);
    this.api.toggleStatus(user.id).subscribe({
      next: () => {
        this.togglingId.set(null);
        this.toast.success('Success', 'User status updated.');
        this.loadUsers();
      },
      error: () => {
        this.togglingId.set(null);
        this.toast.error('Error', 'Failed to update status.');
      },
    });
  }

  protected deleteUser(user: AdminUser): void {
    if (this.deletingId()) return;
    this.deletingId.set(user.id);
    this.api.delete(user.id).subscribe({
      next: () => {
        this.deletingId.set(null);
        this.toast.success('Success', 'User deleted successfully.');
        this.loadUsers();
      },
      error: () => {
        this.deletingId.set(null);
        this.toast.error('Error', 'Failed to delete user.');
      },
    });
  }

  protected isBusy(id: number): boolean {
    return this.togglingId() === id || this.deletingId() === id;
  }

  private loadUsers(): void {
    this.loading.set(true);
    this.api.getAll(this.includeDeleted()).subscribe({
      next: (res) => {
        this.groups.set(res.data ?? null);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  private setPasswordValidators(required: boolean): void {
    const control = this.form.controls.password;
    if (required) {
      control.setValidators([Validators.required, Validators.minLength(8)]);
    } else {
      control.clearValidators();
    }
    control.updateValueAndValidity();
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
