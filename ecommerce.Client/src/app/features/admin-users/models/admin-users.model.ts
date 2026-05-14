import { RegisterAddressRequest } from '../../auth/models/auth.model';

export interface AdminUser {
  id: number;
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  active: boolean;
  deleted: boolean;
}

export interface AdminUsersByRoleResponse {
  admins: AdminUser[];
  merchants: AdminUser[];
  customers: AdminUser[];
}

export interface CreateAdminUserRequest {
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  password: string;
  address: RegisterAddressRequest;
}

export interface UpdateAdminUserRequest {
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  address: RegisterAddressRequest;
}
