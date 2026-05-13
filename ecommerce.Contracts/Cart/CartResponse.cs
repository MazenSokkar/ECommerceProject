using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Contracts.Cart
{
    public record CartResponse(
     int Id,
     List<CartItemResponse> Items,
     decimal Total,
     int TotalItems
 );

    public record CartItemResponse(
        int Id,
        int ProductId,
        string ProductName,
        string? ProductImage,
        decimal UnitPrice,
        int Quantity,
        decimal Subtotal
    );
}
