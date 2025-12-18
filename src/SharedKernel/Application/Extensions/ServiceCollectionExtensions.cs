using System.Configuration;
using Microsoft.Extensions.Configuration;
using SharedKernel.Model;

namespace SharedKernel;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Event API", Version = "v1" });

            var servicesOptions = new ServicesOptions();
           configuration.GetSection("Services").Bind(servicesOptions);
            var issuer = servicesOptions.Auth.BaseUrl;
           //OAuth2/OpenID Connect configuration for Authorization Code flow (PKCE is handled by Swagger UI)
           options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        // Use absolute URLs if Swagger UI is hosted separately; otherwise, relative is fine
                        AuthorizationUrl = new Uri($"{issuer}/connect/authorize"),
                        TokenUrl = new Uri($"{issuer}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect scope" },
                            { "profile", "User profile" },
                            { "email", "User email" },
                            {"web-api","Access to the Web API" }
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
                    new List<string> { "openid", "profile", "email", "web-api" }
                }
            });
        });

        return services;
    }

    public static void UseSwaggerWithOAuth(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API v1");

            options.OAuthScopes("web-api");
            options.OAuthClientId("web-api");
        });
    }
}




