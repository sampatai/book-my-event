using System.Reflection;
using Application;
using Application.Abstractions.Messaging;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Database.Seed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SharedKernel;
using Web.Api;
using Web.Api.Extensions;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;

const string ConnectionStringName = "DefaultConnection";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// Keep and modify the UseWolverine configuration
builder.Host.UseWolverine(opts =>
{
    opts.UseFluentValidation();
    opts.UseEntityFrameworkCoreTransactions();

    string connectionString = builder.Configuration.GetConnectionString(ConnectionStringName)
        ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' not found.");

    opts.PersistMessagesWithPostgresql(connectionString, schemaName: Schemas.Wolverine);
    opts.Durability.Mode = DurabilityMode.Solo;

    // Discover handlers and middleware from other assemblies
    opts.Discovery.IncludeAssembly(typeof(ICommandHandler<>).Assembly);
    opts.Discovery.IncludeAssembly(typeof(Infrastructure.DependencyInjection).Assembly);

    opts.Policies.AddMiddleware(typeof(LoggingMiddleware<>));
});

// This extension method will now contain the full OpenAPI/Swagger configuration.
builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

// It's best practice to only expose Swagger in the development environment for security.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Add this line to generate the swagger.json file
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");

        // When using OpenIddict/OAuth2, you can configure the UI for a seamless login.
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
        c.OAuthUsePkce();
    });

    //app.ApplyMigrations<ApplicationDbContext>();
    //await DbSeeder.SeedOpenIddictClientsAsync(app.Services);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

namespace Web.Api
{
    // Make the auto-generated Program class public so it can be referenced by test projects.
    public partial class Program { }
}
