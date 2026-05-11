using System;

namespace ecommerce.Core.Entities;

public class City : AuditableEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public Country Country { get; set; }
    public int StateProvinceId { get; set; }
    public StateProvince StateProvince { get; set; }
    public bool Active { get; set; }
    public bool Deleted { get; set; }
}
