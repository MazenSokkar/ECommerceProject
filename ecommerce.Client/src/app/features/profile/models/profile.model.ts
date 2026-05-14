import { AddressResponse } from '../../../shared/models/location.model';
import { RegisterAddressRequest } from '../../auth/models/auth.model';

export interface UserProfileResponse {
  id: number;
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  active: boolean;
  addresses: AddressResponse[];
}

export interface UpdateUserProfileRequest {
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  address: RegisterAddressRequest;
}
