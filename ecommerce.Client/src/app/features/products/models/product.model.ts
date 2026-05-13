export interface Product {
    id: number;
    name: string;
    price: number;
    stock: number;
    isActive: boolean;
    primaryImageUrl?: string;
    averageRating: number;
    reviewCount: number;
    categoryName: string;
    merchantStoreName: string;
}

export interface ProductDetails {
    id: number;
    merchantId: number;
    merchantStoreName: string;
    categoryId: number;
    categoryName: string;
    name: string;
    description?: string;
    price: number;
    stock: number;
    isActive: boolean;
    averageRating: number;
    reviewCount: number;
    images: string[];
    createdAt: string;
}

export interface ProductListResponse {
    items: Product[];
    total: number;
}

export interface ProductFilter {
    search?: string;
    categoryId?: number;
    minPrice?: number;
    maxPrice?: number;
    page: number;
    pageSize: number;
    sortBy: 'newest' | 'price_asc' | 'price_desc';
}

export interface CreateProductRequest {
    name: string;
    description?: string;
    price: number;
    stock: number;
    categoryId: number;
}

export interface UpdateProductRequest extends CreateProductRequest {
    isActive: boolean;
}