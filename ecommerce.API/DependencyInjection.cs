using System.Reflection;
using System.Text;
using ecommerce.Contracts;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using ecommerce.Infrastructure.Data;
using ecommerce.Infrastructure.Options;
using ecommerce.Infrastructure.Repositories;
using ecommerce.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Stripe;
using ecommerce.Infrastructure.Services;
using OrderService = ecommerce.Infrastructure.Services.OrderService;
using ProductService = ecommerce.Infrastructure.Services.ProductService;
using ReviewService = ecommerce.Infrastructure.Services.ReviewService;

namespace ecommerce.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services
            .AddCorsConfig()
            .AddOpenApiConfig()
            .AddEntitiesServicesDependencies(configuration)
            .AddEntitiesRepositriesDependencies()
            .AddMapsterConfig()
            .AddSerilogConfig()
            .AddFluentValidationConfig();

        return services;
    }

    public static IServiceCollection AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
            )
        );

        return services;
    }

    public static IServiceCollection AddOpenApiConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddEntitiesRepositriesDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IStateProvinceRepository, StateProvinceRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IMerchantRepository, MerchantRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IShippingRepository, ShippingRepository>();
        services.AddScoped<IShippingService, ShippingService>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentService, PaymentService>();




        return services;
    }

    public static IServiceCollection AddEntitiesServicesDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.Configure<AuthSettings>(configuration.GetSection("AuthSettings"));
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
        Stripe.StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"]!;

        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminUserService, AdminUserService>();

        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IStateProvinceService, StateProvinceService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMerchantService, MerchantService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IWishlistService, WishlistService>();






        return services;
    }

    public static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }

    public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
        services.AddFluentValidationAutoValidation();

        return services;
    }

    public static IServiceCollection AddSerilogConfig(this IServiceCollection services)
    {
        services.AddSerilog((sp, lc) =>
            lc.ReadFrom.Configuration(sp.GetRequiredService<IConfiguration>())
        );


        return services;
    }
}
