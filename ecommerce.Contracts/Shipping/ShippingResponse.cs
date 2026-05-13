using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Contracts.Shipping
{
    public record ShippingResponse(
       int Id,
       int OrderId,
       string? TrackingNumber,
       string? Carrier,
       string Status,
       DateTime? ShippedAt,
       DateTime? DeliveredAt
   );
}
