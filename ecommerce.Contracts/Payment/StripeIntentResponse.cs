using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Contracts.Payment
{
    public record StripeIntentResponse(string ClientSecret, string PublishableKey);

}
