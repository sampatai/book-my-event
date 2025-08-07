namespace SharedKernel;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(static options =>
        {
            //string url = "https://localhost:5001/swagger/oauth2-redirect.html";
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Book My Event V1");
            options.OAuthClientId("swagger-ui"); // Must match your OpenIddict client_id for Swagger UI
            options.OAuthAppName("Swagger UI for API");
            options.OAuthUsePkce(); // Recommended for public clients

            // Set the scopes you want to request
            options.OAuthScopes("openid", "profile", "email");

            // Optional: If your redirect URI is custom, set it here
            //options.OAuth2RedirectUrl(url);


            // Optional: For PKCE-only, disable basic auth with code grant
            //options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        });

        return app;
    }
}
