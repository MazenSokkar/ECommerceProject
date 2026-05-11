namespace ecommerce.Contracts.StateProvince;

public record UpdateStateProvinceRequest(string NameAr, string NameEn, int CountryId, bool Active);
