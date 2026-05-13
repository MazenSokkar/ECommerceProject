import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryApiService {
    private readonly http = inject(HttpClient);
    private readonly base = `${environment.apiUrl}/categories`;

    getAll(): Observable<ApiResponse<Category[]>> {
        return this.http.get<ApiResponse<Category[]>>(this.base);
    }

    getById(id: number): Observable<ApiResponse<Category>> {
        return this.http.get<ApiResponse<Category>>(`${this.base}/${id}`);
    }

    create(request: CreateCategoryRequest): Observable<ApiResponse<Category>> {
        return this.http.post<ApiResponse<Category>>(this.base, request);
    }

    update(id: number, request: UpdateCategoryRequest): Observable<ApiResponse<Category>> {
        return this.http.put<ApiResponse<Category>>(`${this.base}/${id}`, request);
    }

    delete(id: number): Observable<ApiResponse<null>> {
        return this.http.delete<ApiResponse<null>>(`${this.base}/${id}`);
    }
}