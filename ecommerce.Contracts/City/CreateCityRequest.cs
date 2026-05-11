namespace ecommerce.Contracts.City;

public record CreateCityRequest(string NameAr, string NameEn, int CountryId, int StateProvinceId);
