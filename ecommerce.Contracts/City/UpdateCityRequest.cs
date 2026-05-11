namespace ecommerce.Contracts.City;

public record UpdateCityRequest(string NameAr, string NameEn, int CountryId, int StateProvinceId, bool Active);
