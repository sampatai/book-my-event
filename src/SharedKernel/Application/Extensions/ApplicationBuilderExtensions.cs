namespace SharedKernel;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            options.OAuthClientId("swagger-ui");
            options.OAuthAppName("Swagger UI for API");
            options.OAuthUsePkce(); // Recommended for security
        });

        return app;
    }
}
