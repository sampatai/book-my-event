using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using SharedKernel.Model;

namespace Auth.Infrastructure.Data;

public static class OpenIddictSeeder
{
    public static async Task SeedClientsAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var servicesOptions = new ServicesOptions();
        configuration.GetSection("Services").Bind(servicesOptions);

        var webApiScope = OpenIddictConstants.Permissions.Prefixes.Scope + "web-api";
        var reactScope = OpenIddictConstants.Permissions.Prefixes.Scope + "react-app";

        const string webApiClientId = "web-api";

        if (await manager.FindByClientIdAsync(webApiClientId, cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                ClientId = webApiClientId,
                DisplayName = "Swagger UI - Web API",
                RedirectUris =
                {
                    new Uri($"{servicesOptions.WebApi.BaseUrl}/swagger/oauth2-redirect.html")
                },
                PostLogoutRedirectUris =
                {
                    new Uri($"{servicesOptions.WebApi.BaseUrl}/signout-callback-oidc")
                },
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
                }
            }, cancellationToken);
        }

        const string reactClientId = "react-client";

        if (await manager.FindByClientIdAsync(reactClientId, cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor {
                ClientId = reactClientId,
                DisplayName = "React client",
                RedirectUris =
                {
                    new Uri($"{servicesOptions.ReactClient.BaseUrl}/signin-oidc"),
                    new Uri($"{servicesOptions.ReactClient.BaseUrl}/callback"),
                    new Uri($"{servicesOptions.ReactClient.BaseUrl}/")
                },
                PostLogoutRedirectUris =
                {
                    new Uri($"{servicesOptions.ReactClient.BaseUrl}/signout-callback-oidc"),
                    new Uri($"{servicesOptions.ReactClient.BaseUrl}/")
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    reactScope
                }
            }, cancellationToken);
        }
    }
}
