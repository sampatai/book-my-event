using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>(); // Access configuration    

            // Retrieve the URI from configuration    
            var swaggerRedirectUri = configuration.GetValue<string>("OpenIddict:SwaggerRedirectUri");

            // Example: Register a client for Swagger UI or your web app    
            if (await manager.FindByClientIdAsync("swagger-ui") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                    ClientId = "swagger-ui",
                    DisplayName = "Swagger UI",
                    RedirectUris = { new Uri(swaggerRedirectUri!) },
                    Permissions =
                          {
                              OpenIddictConstants.Permissions.Endpoints.Authorization,
                              OpenIddictConstants.Permissions.Endpoints.Token,
                              OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                              OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                              OpenIddictConstants.Permissions.ResponseTypes.Code,
                              OpenIddictConstants.Scopes.OpenId, // Corrected reference to OpenId scope  
                              OpenIddictConstants.Scopes.Email,
                              OpenIddictConstants.Scopes.Profile
                          },
                    Requirements =
                          {
                              OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                          }
                });
            }
        }
    }
}

