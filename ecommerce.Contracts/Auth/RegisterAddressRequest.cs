namespace ecommerce.Contracts.Auth;

public record RegisterAddressRequest(
    string LocationName,
    int CityId,
    int StateProvinceId,
    int CountryId,
    string? Longitude,
    string? Latitude
);
