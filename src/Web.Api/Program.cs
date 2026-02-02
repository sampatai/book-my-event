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
using OpenIddict.Validation;
using OpenIddict.Validation.AspNetCore;
using Serilog;
using SharedKernel;
using SharedKernel.Model;
using Web.Api;
using Web.Api.Extensions;



WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

// This extension method will now contain the full OpenAPI/Swagger configuration.
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);
builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);
builder.Services.AddOpenIddict()
.AddValidation(options =>
 {
     var servicesOptions = new ServicesOptions();
     builder.Configuration.GetSection("Services").Bind(servicesOptions);

     options.SetIssuer(servicesOptions.Auth.BaseUrl);

     options.UseSystemNetHttp();
     options.UseAspNetCore();
 });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
//builder.Services.AddAuthorization(options => options.AddPolicy("ApiScope", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        policy.RequireClaim("scope", "web-api");
//    }));

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
    app.UseDeveloperExceptionPage();


    //app.ApplyMigrations<ApplicationDbContext>();
    //await DbSeeder.SeedOpenIddictClientsAsync(app.Services);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

app.MapControllers();

await app.RunAsync();

namespace Web.Api
{
    // Make the auto-generated Program class public so it can be referenced by test projects.
    public partial class Program { }
}
