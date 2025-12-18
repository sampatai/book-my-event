using System.Configuration;
using System.Reflection;
using System.Security.Claims;
using Application;
using Application.Abstractions.Messaging;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SharedKernel;
using SharedKernel.Model;
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
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);
builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        var servicesOptions = new ServicesOptions();
        builder.Configuration.GetSection("Services").Bind(servicesOptions);
        options.Authority = servicesOptions.Auth.BaseUrl;
        options.MapInboundClaims = true;

        options.BackchannelHttpHandler = new HttpClientHandler();
        // To debug various token validation issues, use events
        options.Events = new JwtBearerEvents {
            // OnAuthenticationFailed = context =>
            // {
            //     var error = context.Exception;
            //     return Task.CompletedTask;
            // },
            // OnForbidden = context =>
            // {
            //     var request = context.Request;
            //     return Task.CompletedTask;
            // },
            OnTokenValidated = async (context) =>
            {
                // OpenIddict sends scope as one claim with space separated values
                // ASP.NET Core validation expects scope claim to be an array of values
                // Split the scope claim to an array to make it pass validation
                // This is only an issue when multiple scopes are used
                // https://stackoverflow.com/questions/54852094/asp-net-core-requireclaim-scope-with-multiple-scopes
                if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                {
                    var scopeClaims = claimsIdentity.FindFirst("scope");
                    if (scopeClaims is not null)
                    {
                        claimsIdentity.RemoveClaim(scopeClaims);
                        claimsIdentity.AddClaims(scopeClaims.Value.Split(' ').Select(scope => new Claim("scope", scope)));
                    }
                }
                await Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateAudience = true,
            ValidAudience = servicesOptions.Auth.BaseUrl, // or the expected audience
            ValidateIssuer = true,
            ValidIssuer = servicesOptions.WebApi.BaseUrl,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddAuthorization(options => options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "web-api");
    }));

// CORS policy to allow SwaggerUI and React clients
builder.Services.AddCors(options => options.AddPolicy("default", policy =>
{
    var servicesOptions = new ServicesOptions();
    builder.Configuration.GetSection("Services").Bind(servicesOptions);
    policy.WithOrigins(
            
            servicesOptions.ReactClient.BaseUrl)
        .AllowAnyHeader()
        .AllowAnyMethod();
}));


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
    app.UseSwaggerWithOAuth(app.Configuration);

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
