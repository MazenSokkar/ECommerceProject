import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../services/auth-api.service';
import { LocationApiService } from '../../../../core/services/location-api.service';
import {
  CityResponse,
  CountryResponse,
  StateProvinceResponse,
} from '../../../../shared/models/location.model';
import { ToastService } from '../../../../core/services/toast.service';
import { RegisterRequest } from '../../models/auth.model';
import { Card } from '../../../../shared/components/card/card';
import { StepIndicator } from '../../../../shared/components/step-indicator/step-indicator';
import { FormField } from '../../../../shared/components/form-field/form-field';
import { PasswordInput } from '../../../../shared/components/password-input/password-input';
import { Button } from '../../../../shared/components/button/button';

const passwordMatchValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const pw = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return pw && confirm && pw !== confirm ? { passwordMismatch: true } : null;
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, Card, StepIndicator, FormField, PasswordInput, Button],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private readonly authApi = inject(AuthApiService);
  private readonly locationApi = inject(LocationApiService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly currentStep = signal<1 | 2>(1);
  protected readonly loading = signal(false);

  protected readonly countries = signal<CountryResponse[]>([]);
  protected readonly states = signal<StateProvinceResponse[]>([]);
  protected readonly cities = signal<CityResponse[]>([]);
  protected readonly loadingStates = signal(false);
  protected readonly loadingCities = signal(false);

  protected readonly registerSteps = [
    { label: 'Personal Info' },
    { label: 'Address' },
  ];

  protected readonly form = new FormGroup(
    {
      firstName: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      lastName: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      email: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.email],
      }),
      phone: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.pattern(/^(10|11|12|15)\d{8}$/)],
      }),
      password: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.minLength(8)],
      }),
      confirmPassword: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      locationName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      countryId: new FormControl<number | null>(null, { validators: [Validators.required] }),
      stateProvinceId: new FormControl<number | null>(null, { validators: [Validators.required] }),
      cityId: new FormControl<number | null>(null, { validators: [Validators.required] }),
      latitude: new FormControl<string>('', { nonNullable: true }),
      longitude: new FormControl<string>('', { nonNullable: true }),
    },
    { validators: passwordMatchValidator },
  );

  constructor() {
    this.locationApi.getCountries().subscribe({
      next: (res) => this.countries.set(res.data ?? []),
    });
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
      error: () => {
        this.loadingStates.set(false);
      },
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
      error: () => {
        this.loadingCities.set(false);
      },
    });
  }

  protected goToStep2(): void {
    const step1Controls = ['firstName', 'lastName', 'email', 'phone', 'password', 'confirmPassword'];
    step1Controls.forEach((name) => this.form.get(name)?.markAsTouched());
    const step1Invalid = step1Controls.some((name) => this.form.get(name)?.invalid);
    if (step1Invalid || this.form.hasError('passwordMismatch')) return;
    this.currentStep.set(2);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  protected goBack(): void {
    this.currentStep.set(1);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  protected onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.loading()) return;

    const v = this.form.getRawValue();
    const body: RegisterRequest = {
      firstName: v.firstName,
      lastName: v.lastName,
      email: v.email,
      phone: `+20${v.phone}`,
      password: v.password,
      address: {
        locationName: v.locationName,
        countryId: v.countryId!,
        stateProvinceId: v.stateProvinceId!,
        cityId: v.cityId!,
        latitude: v.latitude || null,
        longitude: v.longitude || null,
      },
    };

    this.loading.set(true);
    this.authApi.register(body).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.isSuccess) {
          this.toast.success('Success', 'Registration successful! Please check your email.');
          this.router.navigate(['/confirm-email'], { queryParams: { email: v.email } });
        }
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }
}
