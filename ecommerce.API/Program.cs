using ecommerce.API;
using ecommerce.Contracts.Errors;
using ecommerce.Core.Entities;
using ecommerce.Infrastructure.Data;
using ecommerce.Infrastructure.Options;
using ecommerce.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
    o.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var iatString = context.Principal?.FindFirst(JwtRegisteredClaimNames.Iat)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                var isBlacklisted = await cache.GetStringAsync($"Blacklist:{jti}");
                if (!string.IsNullOrEmpty(isBlacklisted))
                {
                    context.Fail("Token is blacklisted.");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(userId) && long.TryParse(iatString, out var iat))
            {
                var revokedBeforeString = await cache.GetStringAsync($"SessionRevokedBefore:{userId}");
                if (!string.IsNullOrEmpty(revokedBeforeString) && long.TryParse(revokedBeforeString, out var revokedBefore))
                {
                    if (iat < revokedBefore)
                    {
                        context.Fail("Token was issued before a global session revocation.");
                        return;
                    }
                }
            }
        }
    };
});
builder.Services.AddApiDependencies(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// error handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechShop API v1"));


app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
