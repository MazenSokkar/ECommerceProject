using System;
using Microsoft.AspNetCore.Identity;

namespace ecommerce.Core.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public bool Deleted { get; set; }


    // navigation properties
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
