using System.Text;
using Application.Abstractions.Authentication;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using SharedKernel;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
       this IServiceCollection services,
       IConfiguration configuration) =>
       services
           .AddServices()
           .AddDatabase(configuration)
           .AddHealthChecks(configuration)
           //.AddAuthenticationInternal(configuration)
           .AddAuthorizationInternal()
           .AddOpenIddictInternal(); // Ensure AddOpenIddictInternal is invoked here

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();


        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention();
                // This sets the default tracking behavior to NoTracking
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped);

        services.AddDataProtection();

        services.AddIdentityCore<User>(options =>
        {
            // Password configuration
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            // For email confirmation
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddRoles<IdentityRole<long>>() // Use IdentityRole<long>
        .AddRoleManager<RoleManager<IdentityRole<long>>>() // Use RoleManager with long as the key type
        .AddEntityFrameworkStores<ApplicationDbContext>() // Provide our context
        .AddSignInManager<SignInManager<User>>() // Use SignInManager
        .AddUserManager<UserManager<User>>() // Use UserManager to create users
        .AddDefaultTokenProviders(); // Enable token providers for email confirmation
        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!);

        return services;
    }

    //private static IServiceCollection AddAuthenticationInternal(
    //    this IServiceCollection services,
    //    IConfiguration configuration)
    //{
    //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //        .AddJwtBearer(o =>
    //        {
    //            o.RequireHttpsMetadata = true;

    //            o.TokenValidationParameters = new TokenValidationParameters
    //            {
    //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
    //                ValidIssuer = configuration["Jwt:Issuer"],
    //                ValidAudience = configuration["Jwt:Audience"],
    //                ClockSkew = TimeSpan.Zero
    //            };
    //        });

    //    services.AddHttpContextAccessor();
    //    services.AddScoped<IUserContext, UserContext>();
    //    services.AddSingleton<IPasswordHasher, PasswordHasher>();
    //    services.AddSingleton<ITokenProvider, TokenProvider>();

    //    return services;
    //}

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthentication(options => options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    private static IServiceCollection AddOpenIddictInternal(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(options => options.UseEntityFrameworkCore()
                .UseDbContext<ApplicationDbContext>())
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize");
                options.SetTokenEndpointUris("/connect/token");
              

                options.AllowAuthorizationCodeFlow();
                options.AllowRefreshTokenFlow();

                // Register the signing and encryption credentials
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough();


                // Register your scopes
                options.RegisterScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile);

                // (Optional) Require PKCE for public clients
                options.RequireProofKeyForCodeExchange();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }

}
