using System;

namespace ecommerce.Core.Entities;

public class Address : AuditableEntity
{
    public int Id { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int? MerchantId { get; set; }
    public Merchant? Merchant { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
    public int StateProvinceId { get; set; }
    public StateProvince StateProvince { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    public string Longitude { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
}
