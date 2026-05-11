export interface CountryResponse {
  id: number;
  nameAr: string;
  nameEn: string;
  active: boolean;
}

export interface StateProvinceResponse {
  id: number;
  nameAr: string;
  nameEn: string;
  countryId: number;
  active: boolean;
}

export interface CityResponse {
  id: number;
  nameAr: string;
  nameEn: string;
  countryId: number;
  stateProvinceId: number;
  active: boolean;
}

export interface AddressResponse {
  id: number;
  locationName: string;
  cityId: number;
  stateProvinceId: number;
  countryId: number;
  longitude: string | null;
  latitude: string | null;
  userId: number | null;
  merchantId: number | null;
}

export interface CreateCountryRequest {
  nameAr: string;
  nameEn: string;
}

export interface UpdateCountryRequest {
  nameAr: string;
  nameEn: string;
  active: boolean;
}

export interface CreateStateProvinceRequest {
  nameAr: string;
  nameEn: string;
  countryId: number;
}

export interface UpdateStateProvinceRequest {
  nameAr: string;
  nameEn: string;
  countryId: number;
  active: boolean;
}

export interface CreateCityRequest {
  nameAr: string;
  nameEn: string;
  countryId: number;
  stateProvinceId: number;
}

export interface UpdateCityRequest {
  nameAr: string;
  nameEn: string;
  countryId: number;
  stateProvinceId: number;
  active: boolean;
}

export interface CreateAddressRequest {
  locationName: string;
  cityId: number;
  stateProvinceId: number;
  countryId: number;
  longitude: string | null;
  latitude: string | null;
  userId: number | null;
  merchantId: number | null;
}

export interface UpdateAddressRequest {
  locationName: string;
  cityId: number;
  stateProvinceId: number;
  countryId: number;
  longitude: string | null;
  latitude: string | null;
}
