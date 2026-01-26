using Microsoft.OpenApi;
using SharedKernel.Model;


namespace Web.Api.Extensions;

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

            // OAuth2 / OpenID Connect configuration for Authorization Code flow (PKCE handled by Swagger UI)
            options.AddSecurityDefinition("web-api", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{issuer}/connect/authorize"),
                        TokenUrl = new Uri($"{issuer}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "OpenID Connect scope",
                            ["profile"] = "User profile",
                            ["email"] = "User email",
                            ["web-api"] = "Access to the Web API"
                        }
                    }
                }
            });

            // Require the defined web-api scheme for all endpoints
            var oauthScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Scheme = "oauth2",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{issuer}/connect/authorize"),
                        TokenUrl = new Uri($"{issuer}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "OpenID Connect scope",
                            ["profile"] = "User profile",
                            ["email"] = "User email",
                            ["web-api"] = "Access to the Web API"
                        }
                    }
                }
            };

            options.AddSecurityDefinition("web-api", oauthScheme);

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
            options.OAuthUsePkce();
        });
    }
}




