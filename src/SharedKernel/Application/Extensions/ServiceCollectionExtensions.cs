namespace SharedKernel;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows {
                    AuthorizationCode = new OpenApiOAuthFlow {
                        AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
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




