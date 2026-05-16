import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import {
    CreateProductRequest,
    Product,
    ProductDetails,
    ProductFilter,
    ProductListResponse,
    UpdateProductRequest,
} from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductApiService {
    private readonly http = inject(HttpClient);
    private readonly base = `${environment.apiUrl}/products`;

    getAll(filter: ProductFilter): Observable<ApiResponse<ProductListResponse>> {
        return this.http.get<ApiResponse<ProductListResponse>>(this.base, {
            params: {
                ...(filter.search && { search: filter.search }),
                ...(filter.categoryId && { categoryId: filter.categoryId }),
                ...(filter.minPrice && { minPrice: filter.minPrice }),
                ...(filter.maxPrice && { maxPrice: filter.maxPrice }),
                page: filter.page,
                pageSize: filter.pageSize,
                sortBy: filter.sortBy,
            },
        });
    }

    getById(id: number): Observable<ApiResponse<ProductDetails>> {
        return this.http.get<ApiResponse<ProductDetails>>(`${this.base}/${id}`);
    }

    getMyProducts(): Observable<ApiResponse<Product[]>> {
        return this.http.get<ApiResponse<Product[]>>(`${this.base}/my-products`);
    }

    create(request: CreateProductRequest): Observable<ApiResponse<ProductDetails>> {
        return this.http.post<ApiResponse<ProductDetails>>(this.base, request);
    }

    update(id: number, request: UpdateProductRequest): Observable<ApiResponse<ProductDetails>> {
        return this.http.put<ApiResponse<ProductDetails>>(`${this.base}/${id}`, request);
    }

    delete(id: number): Observable<ApiResponse<null>> {
        return this.http.delete<ApiResponse<null>>(`${this.base}/${id}`);
    }

    updateStock(id: number, stock: number): Observable<ApiResponse<null>> {
        return this.http.put<ApiResponse<null>>(`${this.base}/${id}/stock`, { stock });
    }
}