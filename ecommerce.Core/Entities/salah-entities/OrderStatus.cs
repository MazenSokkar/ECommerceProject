using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Core.Entities.salah_entities
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
