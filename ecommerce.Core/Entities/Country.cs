using System;

namespace ecommerce.Core.Entities;

public class Country : AuditableEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool Active { get; set; }
    public bool Deleted { get; set; }
}