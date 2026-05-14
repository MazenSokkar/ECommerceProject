using ecommerce.Contracts.Address;
using ecommerce.Contracts.Auth;
using ecommerce.Contracts.Categories;
using ecommerce.Contracts.City;
using ecommerce.Contracts.Country;
using ecommerce.Contracts.Products;
using ecommerce.Contracts.Sellers;
using ecommerce.Contracts.StateProvince;
using ecommerce.Contracts.Users;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using ecommerce.Infrastructure.Repositories;
using ecommerce.Infrastructure.Services;
using Mapster;

namespace ecommerce.API.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.PhoneNumber, src => src.Phone)
            .Map(dest => dest.UserName, src => src.Email);

        // Country
        config.NewConfig<CreateCountryRequest, Country>();
        config.NewConfig<UpdateCountryRequest, Country>();
        config.NewConfig<Country, CountryResponse>();

        // StateProvince
        config.NewConfig<CreateStateProvinceRequest, StateProvince>();
        config.NewConfig<UpdateStateProvinceRequest, StateProvince>();
        config.NewConfig<StateProvince, StateProvinceResponse>();

        // City
        config.NewConfig<CreateCityRequest, City>();
        config.NewConfig<UpdateCityRequest, City>();
        config.NewConfig<City, CityResponse>();

        // Address
        config.NewConfig<CreateAddressRequest, Address>();
        config.NewConfig<UpdateAddressRequest, Address>();
        config.NewConfig<Address, AddressResponse>();

        config.NewConfig<RegisterAddressRequest, Address>()
            .Map(dest => dest.Longitude, src => src.Longitude ?? string.Empty)
            .Map(dest => dest.Latitude, src => src.Latitude ?? string.Empty);

        // Category
        config.NewConfig<CreateCategoryRequest, Category>();
        config.NewConfig<UpdateCategoryRequest, Category>();
        config.NewConfig<Category, CategoryResponse>()
            .Map(dest => dest.Children, src => src.Children);
        //Marchant
        config.NewConfig<CreateMerchantRequest, Merchant>();
        config.NewConfig<UpdateMerchantRequest, Merchant>();
        config.NewConfig<Merchant, MerchantResponse>();

        // Product
        config.NewConfig<CreateProductRequest, Product>();
        config.NewConfig<UpdateProductRequest, Product>();

        //Review 


        //WishList
        config.NewConfig<ApplicationUser, AdminUserResponse>()
            .Map(dest => dest.Phone, src => src.PhoneNumber ?? string.Empty)
            .Map(dest => dest.Email, src => src.Email ?? string.Empty);

    }
}
