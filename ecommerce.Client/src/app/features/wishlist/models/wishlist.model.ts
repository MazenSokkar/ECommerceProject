export interface WishlistItem {
    id: number;
    productId: number;
    productName: string;
    productPrice: number;
    productImage?: string;
    addedAt: string;
}

export interface Wishlist {
    id: number;
    items: WishlistItem[];
}

export interface AddToWishlistRequest {
    productId: number;
}