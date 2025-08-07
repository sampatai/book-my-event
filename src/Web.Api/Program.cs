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

 

    // Fix for CS0305: Specify the generic type argument for ICommandHandler<TCommand>  
    opts.Discovery.IncludeAssembly(typeof(ICommandHandler<>).Assembly);
    opts.Discovery.IncludeAssembly(typeof(Infrastructure.DependencyInjection).Assembly);

    opts.Policies.AddMiddleware(typeof(LoggingMiddleware<>));
});


builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
    app.ApplyMigrations<ApplicationDbContext>();
    await DbSeeder.SeedOpenIddictClientsAsync(app.Services); // <-- Add this line
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

namespace Web.Api
{
    public partial class Program { }
}