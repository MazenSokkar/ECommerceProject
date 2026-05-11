import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';
import {
  AddressResponse,
  CityResponse,
  CountryResponse,
  CreateAddressRequest,
  CreateCityRequest,
  CreateCountryRequest,
  CreateStateProvinceRequest,
  StateProvinceResponse,
  UpdateAddressRequest,
  UpdateCityRequest,
  UpdateCountryRequest,
  UpdateStateProvinceRequest,
} from '../../shared/models/location.model';

// ── Service ───────────────────────────────────────────────────────────────────

@Injectable({ providedIn: 'root' })
export class LocationApiService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  // Countries
  getCountries(): Observable<ApiResponse<CountryResponse[]>> {
    return this.http.get<ApiResponse<CountryResponse[]>>(`${this.base}/countries`);
  }

  getCountryById(id: number): Observable<ApiResponse<CountryResponse>> {
    return this.http.get<ApiResponse<CountryResponse>>(`${this.base}/countries/${id}`);
  }

  createCountry(request: CreateCountryRequest): Observable<ApiResponse<CountryResponse>> {
    return this.http.post<ApiResponse<CountryResponse>>(`${this.base}/countries`, request);
  }

  updateCountry(
    id: number,
    request: UpdateCountryRequest,
  ): Observable<ApiResponse<CountryResponse>> {
    return this.http.put<ApiResponse<CountryResponse>>(`${this.base}/countries/${id}`, request);
  }

  deleteCountry(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/countries/${id}`);
  }

  // State Provinces
  getStateProvinces(): Observable<ApiResponse<StateProvinceResponse[]>> {
    return this.http.get<ApiResponse<StateProvinceResponse[]>>(`${this.base}/state-provinces`);
  }

  getStateProvincesByCountry(countryId: number): Observable<ApiResponse<StateProvinceResponse[]>> {
    return this.http.get<ApiResponse<StateProvinceResponse[]>>(
      `${this.base}/state-provinces/by-country/${countryId}`,
    );
  }

  getStateProvinceById(id: number): Observable<ApiResponse<StateProvinceResponse>> {
    return this.http.get<ApiResponse<StateProvinceResponse>>(`${this.base}/state-provinces/${id}`);
  }

  createStateProvince(
    request: CreateStateProvinceRequest,
  ): Observable<ApiResponse<StateProvinceResponse>> {
    return this.http.post<ApiResponse<StateProvinceResponse>>(
      `${this.base}/state-provinces`,
      request,
    );
  }

  updateStateProvince(
    id: number,
    request: UpdateStateProvinceRequest,
  ): Observable<ApiResponse<StateProvinceResponse>> {
    return this.http.put<ApiResponse<StateProvinceResponse>>(
      `${this.base}/state-provinces/${id}`,
      request,
    );
  }

  deleteStateProvince(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/state-provinces/${id}`);
  }

  // Cities
  getCities(): Observable<ApiResponse<CityResponse[]>> {
    return this.http.get<ApiResponse<CityResponse[]>>(`${this.base}/cities`);
  }

  getCitiesByStateProvince(stateProvinceId: number): Observable<ApiResponse<CityResponse[]>> {
    return this.http.get<ApiResponse<CityResponse[]>>(
      `${this.base}/cities/by-state-province/${stateProvinceId}`,
    );
  }

  getCityById(id: number): Observable<ApiResponse<CityResponse>> {
    return this.http.get<ApiResponse<CityResponse>>(`${this.base}/cities/${id}`);
  }

  createCity(request: CreateCityRequest): Observable<ApiResponse<CityResponse>> {
    return this.http.post<ApiResponse<CityResponse>>(`${this.base}/cities`, request);
  }

  updateCity(id: number, request: UpdateCityRequest): Observable<ApiResponse<CityResponse>> {
    return this.http.put<ApiResponse<CityResponse>>(`${this.base}/cities/${id}`, request);
  }

  deleteCity(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/cities/${id}`);
  }

  // Addresses
  getAddresses(): Observable<ApiResponse<AddressResponse[]>> {
    return this.http.get<ApiResponse<AddressResponse[]>>(`${this.base}/addresses`);
  }

  getAddressesByUser(userId: number): Observable<ApiResponse<AddressResponse[]>> {
    return this.http.get<ApiResponse<AddressResponse[]>>(
      `${this.base}/addresses/by-user/${userId}`,
    );
  }

  getAddressesByMerchant(merchantId: number): Observable<ApiResponse<AddressResponse[]>> {
    return this.http.get<ApiResponse<AddressResponse[]>>(
      `${this.base}/addresses/by-merchant/${merchantId}`,
    );
  }

  getAddressById(id: number): Observable<ApiResponse<AddressResponse>> {
    return this.http.get<ApiResponse<AddressResponse>>(`${this.base}/addresses/${id}`);
  }

  createAddress(request: CreateAddressRequest): Observable<ApiResponse<AddressResponse>> {
    return this.http.post<ApiResponse<AddressResponse>>(`${this.base}/addresses`, request);
  }

  updateAddress(
    id: number,
    request: UpdateAddressRequest,
  ): Observable<ApiResponse<AddressResponse>> {
    return this.http.put<ApiResponse<AddressResponse>>(`${this.base}/addresses/${id}`, request);
  }

  deleteAddress(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/addresses/${id}`);
  }
}
