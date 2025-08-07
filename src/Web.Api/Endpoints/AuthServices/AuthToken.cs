


using Domain.Users.Root;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using System.Globalization;
using OpenIddict.Abstractions;

namespace Web.Api.Endpoints.AuthServices;

public class AuthToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/connect/token", async (HttpContext context) =>
        {
            var request = context.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

            var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
            var signInManager = context.RequestServices.GetRequiredService<SignInManager<User>>();
            var user = await userManager.FindByEmailAsync(request.Username ?? string.Empty);
            if (user == null || !await userManager.CheckPasswordAsync(user, request.Password ?? string.Empty))
                return Results.BadRequest(new { error = "invalid_grant", error_description = "Invalid credentials." });

            var principal = await signInManager.CreateUserPrincipalAsync(user);

            // Add custom claims
            if (user.ServiceEntityId is not null)
            {
                var identity = (ClaimsIdentity)principal.Identity!;
                identity.AddClaim(new Claim("tenant_id", user.ServiceEntityId.Value.ToString(CultureInfo.InvariantCulture)));
            }
            if (request.Scope is not null)
            {
                var identity = (ClaimsIdentity)principal.Identity!;
                identity.AddClaim(new Claim("scope", request.Scope));
            }

            return Results.SignIn(
                principal,
                properties: null,
                authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        });

        // Endpoint for OpenID Connect authorization requests
        app.MapGet("/connect/authorize", (HttpContext context) =>
        {
            var request = context.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

            // If the user is not authenticated, redirect to login UI or return 401
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                // Redirect to login page or return Results.Unauthorized()
                return Results.Unauthorized();
            }

            // Use the existing authenticated principal with all claims
            var principal = context.User;

            // Set requested scopes
            principal.SetScopes(request.GetScopes());

            // Issue the authorization code
            return Results.SignIn(principal, properties: null, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }).WithTags(Tags.Auth);
    }
}
