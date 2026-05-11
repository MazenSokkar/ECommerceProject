namespace ecommerce.Contracts.Address;

public record AddressResponse(
    int Id,
    string LocationName,
    int CityId,
    int StateProvinceId,
    int CountryId,
    string Longitude,
    string Latitude,
    int? UserId,
    int? MerchantId);
