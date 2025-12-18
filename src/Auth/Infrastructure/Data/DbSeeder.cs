using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth.Domain.TenantEntity.Root;
using Auth.Domain.Users.Root;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Auth.Infrastructure.Database.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedOpenIddictClientsAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
            var _configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var reactClientUrl = _configuration.GetSection("react-client")!.ToString();
            var webapiUrl = _configuration.GetSection("web-api")!.ToString();
            string webApiScope = Permissions.Prefixes.Scope + "web-api";

            var clientId = "web-api";

            // Web API Client (if acting as a client to another API)
            if (await manager.FindByClientIdAsync(clientId, cancellationToken) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = clientId,
                    DisplayName = "Swagger UI - Web API",
                    RedirectUris = { new Uri($"{webapiUrl}swagger/oauth2-redirect.html") },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                         OpenIddictConstants.Permissions.Scopes.Email,
                         webApiScope
                    },
                    PostLogoutRedirectUris =
                {
                    new Uri($"{webapiUrl}signout-callback-oidc")
                },
                }, cancellationToken);
            }

            clientId = "react-client";
            if (await manager.FindByClientIdAsync(clientId, cancellationToken) == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = clientId,
                    DisplayName = "React client",
                    RedirectUris =
                    {
                    new Uri($"{reactClientUrl}signin-oidc")
                },
                    PostLogoutRedirectUris =
                    {
                    new Uri($"{reactClientUrl}signout-callback-oidc")
                },
                    Permissions =
                    {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Scopes.OpenId,
                    Scopes.OfflineAccess,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    webApiScope
                }
                }, cancellationToken);
            }
            // Seed ServiceEntity
            if (!await db.TenantEntities.AnyAsync(cancellationToken: cancellationToken))
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
                var serviceEntity = new TenantEntity(
                    name: "My Service",
                    description: "A description of the service.",
                    startDate: DateOnly.FromDateTime(DateTime.UtcNow),
                    endDate: null, // or DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
                    image: "https://example.com/image.png",
                    contactEmail: "contact@example.com",
                    phoneNumber: "+1234567890",
                    website: "https://example.com",
                    address: address,
                    Guid.NewGuid()
                );
                db.TenantEntities.Add(serviceEntity);
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

