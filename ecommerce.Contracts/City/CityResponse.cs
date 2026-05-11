namespace ecommerce.Contracts.City;

public record CityResponse(int Id, string NameAr, string NameEn, int CountryId, int StateProvinceId, bool Active);
