import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';
import { Product } from '../../features/products/models/product.model';

@Injectable({
  providedIn: 'root',
})
export class HomeApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/homepage`;

  getBestSellers(count: number = 8): Observable<ApiResponse<Product[]>> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Product[]>>(`${this.baseUrl}/best-sellers`, { params });
  }

  getNewArrivals(count: number = 8): Observable<ApiResponse<Product[]>> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Product[]>>(`${this.baseUrl}/new-arrivals`, { params });
  }

  getFeaturedProducts(count: number = 8): Observable<ApiResponse<Product[]>> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Product[]>>(`${this.baseUrl}/featured`, { params });
  }
}
