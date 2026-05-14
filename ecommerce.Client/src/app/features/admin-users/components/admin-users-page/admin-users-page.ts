import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, computed, inject, signal } from '@angular/core';
import { ToastService } from '../../../../core/services/toast.service';
import { LocationApiService } from '../../../../core/services/location-api.service';
import {
  UserFormModal,
  UserFormValue,
} from '../../../../shared/components/user-form-modal/user-form-modal';
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
  imports: [CommonModule, UserFormModal],
  templateUrl: './admin-users-page.html',
  styleUrl: './admin-users-page.css',
})
export class AdminUsersPage implements OnInit {
  private readonly api = inject(AdminUsersApiService);
  private readonly locationApi = inject(LocationApiService);
  private readonly toast = inject(ToastService);

  @ViewChild(UserFormModal) private modalRef!: UserFormModal;

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly togglingId = signal<number | null>(null);
  protected readonly deletingId = signal<number | null>(null);
  protected readonly includeDeleted = signal(false);
  protected readonly groups = signal<AdminUsersByRoleResponse | null>(null);
  protected readonly activeTab = signal<'customers' | 'admins' | 'merchants'>('customers');
  protected readonly editingUser = signal<AdminUser | null>(null);
  protected readonly modalOpen = signal(false);

  protected readonly isEditing = computed(() => this.editingUser() !== null);

  protected readonly currentUsers = computed(() => {
    const data = this.groups();
    if (!data) return [] as AdminUser[];
    if (this.activeTab() === 'admins') return data.admins;
    if (this.activeTab() === 'merchants') return data.merchants;
    return data.customers;
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  protected changeTab(tab: 'customers' | 'admins' | 'merchants'): void {
    this.activeTab.set(tab);
  }

  protected toggleIncludeDeleted(): void {
    this.includeDeleted.update((v) => !v);
    this.loadUsers();
  }

  protected openCreate(): void {
    this.editingUser.set(null);
    this.modalOpen.set(true);
  }

  protected openEdit(user: AdminUser): void {
    this.editingUser.set(user);
    this.modalOpen.set(true);

    // Load address after modal opens (next tick so ViewChild is ready)
    setTimeout(() => {
      this.locationApi.getAddressesByUser(user.id).subscribe({
        next: (res) => {
          const address = res.data?.[0];
          if (!address || !this.modalRef) return;
          this.modalRef.patchAddress({
            firstName: user.firstName,
            lastName: user.lastName,
            email: user.email,
            phone: this.stripPhone(user.phone),
            locationName: address.locationName,
            countryId: address.countryId,
            stateProvinceId: address.stateProvinceId,
            cityId: address.cityId,
            latitude: address.latitude ?? '',
            longitude: address.longitude ?? '',
          });
        },
      });
    });
  }

  protected closeModal(): void {
    this.editingUser.set(null);
    this.modalOpen.set(false);
  }

  protected onFormSubmitted(value: UserFormValue): void {
    if (this.saving()) return;
    this.saving.set(true);

    if (this.isEditing()) {
      const user = this.editingUser()!;
      const body: UpdateAdminUserRequest = {
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
      this.api.update(user.id, body).subscribe({
        next: () => {
          this.saving.set(false);
          this.toast.success('Success', 'User updated successfully.');
          this.loadUsers();
          this.closeModal();
        },
        error: () => {
          this.saving.set(false);
          this.toast.error('Error', 'Failed to update user.');
        },
      });
      return;
    }

    const body: CreateAdminUserRequest = {
      firstName: value.firstName,
      lastName: value.lastName,
      email: value.email,
      phone: this.normalizePhone(value.phone),
      password: value.password,
      address: {
        locationName: value.locationName,
        countryId: value.countryId!,
        stateProvinceId: value.stateProvinceId!,
        cityId: value.cityId!,
        latitude: value.latitude || null,
        longitude: value.longitude || null,
      },
    };

    this.api.create(body).subscribe({
      next: () => {
        this.saving.set(false);
        this.toast.success('Success', 'User created successfully.');
        this.loadUsers();
        this.closeModal();
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

  protected toggleDeleted(user: AdminUser): void {
    if (this.deletingId()) return;
    this.deletingId.set(user.id);
    this.api.toggleDeleted(user.id).subscribe({
      next: () => {
        this.deletingId.set(null);
        const msg = user.deleted ? 'User restored successfully.' : 'User deleted successfully.';
        this.toast.success('Success', msg);
        this.loadUsers();
      },
      error: () => {
        this.deletingId.set(null);
        this.toast.error('Error', 'Operation failed. Please try again.');
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

  private stripPhone(value: string): string {
    if (!value) return '';
    return value.startsWith('+20') ? value.slice(3) : value;
  }

  private normalizePhone(value: string): string {
    if (!value) return value;
    return value.startsWith('+20') ? value : `+20${value}`;
  }
}
