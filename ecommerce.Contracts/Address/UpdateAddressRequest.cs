namespace ecommerce.Contracts.Address;

public record UpdateAddressRequest(
    string LocationName,
    int CityId,
    int StateProvinceId,
    int CountryId,
    string Longitude,
    string Latitude);
