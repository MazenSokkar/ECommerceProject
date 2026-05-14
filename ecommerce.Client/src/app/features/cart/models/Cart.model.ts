
export interface CartItem {
  id: number;
  productId: number;
  productName: string;
  productImage: string | null;  // كان imageUrl
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface Cart {
  id: number;
  items: CartItem[];
  total: number;        // كان totalAmount
  totalItems: number;   // كان itemsCount
}
export interface AddToCartRequest {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}