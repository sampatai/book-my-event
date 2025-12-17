using Auth;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Quartz;
using OpenIddict.Server;
using Microsoft.Extensions.Configuration;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Enable PII logging for debugging purposes (disable in production)
IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthInfrastructure(builder.Configuration);
// Add services to the container.
builder.Services.AddControllersWithViews();

// OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
// (like pruning orphaned authorizations/tokens from the database) at regular intervals.
builder.Services.AddQuartz(options =>
{
    //options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});
builder.Services.AddOpenIddict()

    // Register the OpenIddict core components.
    .AddCore(options =>
    {
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();

        // Enable Quartz.NET integration.
        options.UseQuartz();
    })

    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        // Enable the authorization, logout, token and userinfo endpoints.
        options
            .SetAuthorizationEndpointUris("connect/authorize")
            .SetTokenEndpointUris("connect/token");


        // Mark the "email", "profile" and "roles" scopes as supported scopes.
        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        // Disable access token encryption, as it is not used in Identity Server sample
        options.DisableAccessTokenEncryption();

        // Enable necessary flows
        options.AllowClientCredentialsFlow();
        options.AllowAuthorizationCodeFlow();
        options.AllowRefreshTokenFlow();

        // Register the signing and encryption credentials.
        // TODO: Use proper certificates for non-development environment
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableStatusCodePagesIntegration();
    })

    // Register the OpenIddict validation components.
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

// CORS policy to allow SwaggerUI and React clients
builder.Services.AddCors(options => options.AddPolicy("default", policy =>
    {
        var webApi = builder.Configuration["Services:web-api"];
        var reactClient = builder.Configuration["Services:react-client"];
        policy.WithOrigins(
                webApi ?? string.Empty,
                reactClient ?? string.Empty)
            .AllowAnyHeader()
            .AllowAnyMethod();
    }));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("default");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages();


await app.RunAsync();
