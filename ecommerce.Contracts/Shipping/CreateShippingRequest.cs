using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Contracts.Shipping
{
    public class CreateShippingRequest
    {
        public int OrderId { get; set; }
        public string? Carrier { get; set; }
    }
}
