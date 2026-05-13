export interface Category {
    id: number;
    name: string;
    slug: string;
    imageUrl?: string;
    parentId?: number;
    children: Category[];
}

export interface CreateCategoryRequest {
    name: string;
    parentId?: number;
    imageUrl?: string;
}

export interface UpdateCategoryRequest {
    name: string;
    parentId?: number;
    imageUrl?: string;
}