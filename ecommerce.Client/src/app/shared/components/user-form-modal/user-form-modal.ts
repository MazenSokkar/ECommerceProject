import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocationApiService } from '../../../core/services/location-api.service';
import { CityResponse, CountryResponse, StateProvinceResponse } from '../../models/location.model';
import { FormField } from '../form-field/form-field';
import { Button } from '../button/button';
import { PasswordInput } from '../password-input/password-input';

export interface UserFormValue {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;
  locationName: string;
  countryId: number | null;
  stateProvinceId: number | null;
  cityId: number | null;
  latitude: string;
  longitude: string;
}

export interface UserFormPatch {
  firstName?: string;
  lastName?: string;
  email?: string;
  phone?: string;
  locationName?: string;
  countryId?: number | null;
  stateProvinceId?: number | null;
  cityId?: number | null;
  latitude?: string;
  longitude?: string;
}

@Component({
  selector: 'app-user-form-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormField, Button, PasswordInput],
  templateUrl: './user-form-modal.html',
  styleUrl: './user-form-modal.css',
})
export class UserFormModal implements OnInit, OnChanges {
  private readonly locationApi = inject(LocationApiService);

  /** Whether the modal is visible */
  @Input() open = false;
  /** Title shown in the modal header */
  @Input() title = 'Create user';
  /** Label for the submit button */
  @Input() submitLabel = 'Create user';
  /** Whether to show the password field */
  @Input() showPassword = true;
  /** Whether a save is in progress */
  @Input() saving = false;
  /** Pre-fill form values (for edit mode) */
  @Input() initialValue: UserFormPatch | null = null;

  @Output() submitted = new EventEmitter<UserFormValue>();
  @Output() cancelled = new EventEmitter<void>();

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

  ngOnInit(): void {
    this.locationApi.getCountries().subscribe({
      next: (res) => this.countries.set(res.data ?? []),
    });
    this.applyPasswordValidators();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open'] && this.open) {
      this.resetForm();
    }
    if (changes['showPassword']) {
      this.applyPasswordValidators();
    }
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
    if (this.form.invalid || this.saving) return;
    this.submitted.emit(this.form.getRawValue() as UserFormValue);
  }

  protected onCancel(): void {
    this.cancelled.emit();
  }

  protected onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.onCancel();
    }
  }

  /** Called by the parent to load address data for edit mode */
  patchAddress(patch: UserFormPatch): void {
    this.form.patchValue(patch);

    if (patch.countryId) {
      this.loadingStates.set(true);
      this.locationApi.getStateProvincesByCountry(patch.countryId).subscribe({
        next: (res) => {
          this.states.set(res.data ?? []);
          this.loadingStates.set(false);
        },
        error: () => this.loadingStates.set(false),
      });
    }

    if (patch.stateProvinceId) {
      this.loadingCities.set(true);
      this.locationApi.getCitiesByStateProvince(patch.stateProvinceId).subscribe({
        next: (res) => {
          this.cities.set(res.data ?? []);
          this.loadingCities.set(false);
        },
        error: () => this.loadingCities.set(false),
      });
    }
  }

  private resetForm(): void {
    this.form.reset();
    this.states.set([]);
    this.cities.set([]);
    this.applyPasswordValidators();

    if (this.initialValue) {
      this.form.patchValue(this.initialValue);
    }
  }

  private applyPasswordValidators(): void {
    const ctrl = this.form.controls.password;
    if (this.showPassword) {
      ctrl.setValidators([Validators.required, Validators.minLength(8)]);
    } else {
      ctrl.clearValidators();
    }
    ctrl.updateValueAndValidity();
  }
}
