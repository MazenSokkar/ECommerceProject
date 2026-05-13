using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Contracts.Cart
{

    public record AddToCartRequest(int ProductId, int Quantity);

    public record UpdateCartItemRequest(int Quantity);
}
