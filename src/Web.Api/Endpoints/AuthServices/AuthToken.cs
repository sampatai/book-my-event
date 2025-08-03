


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

        app.MapPost("/connect/token", async (HttpContext context) => {
            var request = context.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

            // Validate user credentials (replace with your logic)
            var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(request.Username ?? string.Empty);
            if (user == null || !await userManager.CheckPasswordAsync(user, request.Password ?? string.Empty))
                return Results.BadRequest(new { error = "invalid_grant", error_description = "Invalid credentials." });

            // Add tenant claim to support multi-tenancy
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.AddClaim(new Claim(Claims.Subject, user.Id.ToString(CultureInfo.InvariantCulture)));
            identity.AddClaim(new Claim(Claims.Email, user.Email ?? string.Empty));

            //Assume tenant id is a property on User, adjust as needed
            if (user.ServiceEntityId is not null)
            {
                string tenantId = user.ServiceEntityId.Value.ToString(CultureInfo.InvariantCulture);
                identity.AddClaim(new Claim("tenant_id", tenantId));
            }

            // Add scopes as claims if present
            if (request.Scope is not null)
            {
                identity.AddClaim(new Claim("scope", request.Scope));
            }

            var principal = new ClaimsPrincipal(identity);

            // OpenIddict expects authentication properties, not just the scheme string
            return Results.SignIn(
                principal,
                properties: null,
                authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }).WithTags(Tags.Auth);


        // Endpoint for OpenID Connect authorization requests
        app.MapGet("/connect/authorize", (HttpContext context) => {
            var request = context.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

            // If the user is not authenticated, redirect to login UI or return 401
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                // Redirect to login page or return Results.Unauthorized()
                return Results.Unauthorized();
            }

            // Create a ClaimsPrincipal for the authenticated user
            var claims = new List<Claim>
                        {
                            new(Claims.Subject, context.User.FindFirst(Claims.Subject)?.Value ?? ""),
                            new(Claims.Email, context.User.FindFirst(Claims.Email)?.Value ?? "")
                        };

            var identity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Set requested scopes
            principal.SetScopes(request.GetScopes());

            // Issue the authorization code
            return Results.SignIn(principal, properties: null, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }).WithTags(Tags.Auth);
    }
}
