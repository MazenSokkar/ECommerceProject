export interface RegisterAddressRequest {
  locationName: string;
  cityId: number;
  stateProvinceId: number;
  countryId: number;
  longitude?: string | null;
  latitude?: string | null;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  password: string;
  address: RegisterAddressRequest;
}

export interface ConfirmEmailRequest {
  email: string;
  code: string;
}

export interface LoginRequest {
  identifier: string;
  password: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  newPassword: string;
}

export interface AuthResponse {
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
  token: string;
  expiresIn: number;
}

export interface UserProfile {
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}
