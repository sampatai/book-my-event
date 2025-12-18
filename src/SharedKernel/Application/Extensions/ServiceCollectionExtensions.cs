namespace SharedKernel;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            //OAuth2/OpenID Connect configuration for Authorization Code flow (PKCE is handled by Swagger UI)
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        // Use absolute URLs if Swagger UI is hosted separately; otherwise, relative is fine
                        AuthorizationUrl = new Uri($"https://login.yahoo.com/"),
                        TokenUrl = new Uri("/connect/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect scope" },
                            { "profile", "User profile" },
                            { "email", "User email" }
                        }
                    }
                }
            });

            // Require the defined OAuth2 scheme for all endpoints
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    new List<string> { "openid", "profile", "email" }
                }
            });
        });

        return services;
    }
}




