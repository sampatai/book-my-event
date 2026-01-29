using Microsoft.Extensions.DependencyInjection;
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

            // 1. Define the security scheme
            options.AddSecurityDefinition("web-api", new OpenApiSecurityScheme {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows {
                    AuthorizationCode = new OpenApiOAuthFlow {
                        AuthorizationUrl = new Uri($"{issuer}/connect/authorize"),
                        TokenUrl = new Uri($"{issuer}/connect/token"),
                        Scopes = new Dictionary<string, string> {
                            ["openid"] = "OpenID Connect scope",
                            ["profile"] = "User profile",
                            ["email"] = "User email",
                            ["web-api"] = "Access to the Web API"
                        }
                    }
                }
            });

            // 2. Add the security requirement using the NEW delegate pattern
            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement {
                {
                    new OpenApiSecuritySchemeReference("web-api", doc),
                    new List<string> { "web-api" }
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
            options.OAuthUsePkce();
        });
    }
}




