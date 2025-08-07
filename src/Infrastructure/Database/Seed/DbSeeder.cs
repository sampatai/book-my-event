using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.ServiceEntity.Root;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Infrastructure.Database.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedOpenIddictClientsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

            // Web App Client
            var webAppRedirectUri = configuration.GetValue<string>("OpenIddict:WebAppRedirectUri");
            if (await manager.FindByClientIdAsync("web-app-client") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = "web-app-client",
                    DisplayName = "Web Application",
                    RedirectUris = { new Uri(webAppRedirectUri!) },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,

                         OpenIddictConstants.Permissions.Scopes.Profile,
                         OpenIddictConstants.Permissions.Scopes.Email,


                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange // PKCE required
                    }
                });
            }

            // Mobile App Client (using custom URI scheme)
            var mobileRedirectUri = configuration.GetValue<string>("OpenIddict:MobileRedirectUri");
            if (await manager.FindByClientIdAsync("mobile-app-client") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = "mobile-app-client",
                    DisplayName = "Mobile Application",
                    RedirectUris = { new Uri(mobileRedirectUri!) }, // e.g., myapp://callback
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                         OpenIddictConstants.Permissions.Scopes.Email,


                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange // PKCE required
                    }
                });
            }



            // Web API Client (if acting as a client to another API)
            var apiRedirectUri = configuration.GetValue<string>("OpenIddict:ApiRedirectUri");
            if (!string.IsNullOrEmpty(apiRedirectUri) && await manager.FindByClientIdAsync("swagger-ui") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = "swagger-ui",
                    DisplayName = "Web API Client",
                    RedirectUris = { new Uri(apiRedirectUri!) },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                         OpenIddictConstants.Permissions.Scopes.Email,

                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange // PKCE required
                    }
                });
            }
            // Seed ServiceEntity
            if (!await db.ServiceEntities.AnyAsync())
            {
                // Example Address creation (adjust as needed)
                var address = new Address(
                    street: "123 Main St",
                    suburb: "Central",
                    state: "StateName",
                    postcode: "12345",
                    country: "CountryName",
                    city: "CityName"
                );

                // Example ServiceEntity creation
                var serviceEntity = new ServiceEntity(
                    name: "My Service",
                    description: "A description of the service.",
                    startDate: DateOnly.FromDateTime(DateTime.UtcNow),
                    endDate: null, // or DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
                    image: "https://example.com/image.png",
                    contactEmail: "contact@example.com",
                    phoneNumber: "+1234567890",
                    website: "https://example.com",
                    address: address
                );
                db.ServiceEntities.Add(serviceEntity);
                await db.SaveEntitiesAsync(cancellationToken: default);

                // Seed User linked to ServiceEntity
                var user = new User(
                    username: "admin@example.com",
                    firstname: "Admin",
                    lastname: "User",
                   alternativeContact: "+1234567890",
                   email: "admin@example.com",
                   phoneNumber: "+1234567890"
                );


                user.EmailConfirmed = true;
                // Optionally set the address
                user.SetAddress(
                    street: "123 Main St",
                    suburb: "Central",
                    state: "StateName",
                    postcode: "12345",
                    country: "CountryName",
                    city: "CityName"
                );

                // Optionally set the ServiceEntityId if you have one
                user.SetServiceEntityId(serviceEntityId: serviceEntity.Id);
                var result = await userManager.CreateAsync(user, "Admin@1234");
                if (result.Succeeded)
                {
                    // Ensure the Administrator role exists
                    const string adminRole = "Administrator";
                    if (!await roleManager.RoleExistsAsync(adminRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole<long>(adminRole));
                    }

                    // Assign user to Administrator role
                    await userManager.AddToRoleAsync(user, adminRole);

                    // Add custom and global claims
                    var claims = new List<Claim>
                    {
                        new Claim("custom_claim", "custom_value"),
                        new Claim("globaluserclaim", "global_value"),
                        new Claim(ClaimTypes.GivenName, user.FirstName),
                        new Claim(ClaimTypes.Surname, user.LastName)
                    };
                    await userManager.AddClaimsAsync(user, claims);
                }

            }
        }
    }
}

