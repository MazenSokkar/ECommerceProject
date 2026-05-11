using System;
using Microsoft.AspNetCore.Identity;

namespace ecommerce.Core.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public bool IsDefault { get; set; }
    public bool Active { get; set; }
    public bool Deleted { get; set; }
}
