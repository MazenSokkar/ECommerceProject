using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Contracts.Payment
{
    public record CreateStripeIntentRequest(int OrderId);

}
