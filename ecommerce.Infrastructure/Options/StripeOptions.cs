using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Infrastructure.Options
{
    public class StripeOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
    }

}
