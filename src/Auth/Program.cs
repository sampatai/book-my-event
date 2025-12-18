using Auth;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Database.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using OpenIddict.Server;
using Quartz;
using SharedKernel.Model;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Enable PII logging for debugging purposes (disable in production)
IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthInfrastructure(builder.Configuration);
//// Add services to the container.
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
            .UseDbContext<AuthenticationDbContext>();

        // Enable Quartz.NET integration.
        options.UseQuartz();
    })
            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the authorization, logout, token and userinfo endpoints.
                // Enable the authorization, logout, token and userinfo endpoints.
                options.SetAuthorizationEndpointUris("connect/authorize")
                       .SetEndSessionEndpointUris("connect/logout")
                       .SetIntrospectionEndpointUris("connect/introspect")
                       .SetTokenEndpointUris("connect/token")
                       .SetUserInfoEndpointUris("connect/userinfo")
                       .SetEndUserVerificationEndpointUris("connect/verify");

                // Mark the "email", "profile" and "roles" scopes as supported scopes.
                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                // Note: the sample uses the code and refresh token flows but you can enable
                // the other flows if you need to support implicit, password or client credentials.
                options.AllowAuthorizationCodeFlow()
                       .AllowRefreshTokenFlow();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableEndSessionEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserInfoEndpointPassthrough()
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
        var servicesOptions = new ServicesOptions();
        builder.Configuration.GetSection("Services").Bind(servicesOptions);
        policy.WithOrigins(
                servicesOptions.WebApi.BaseUrl,
                servicesOptions.ReactClient.BaseUrl)
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
    await DbSeeder.SeedOpenIddictClientsAsync(app.Services, CancellationToken.None);
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("default");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();
//app.MapRazorPages(); // <--- ADD THIS LINE HERE
app.MapDefaultControllerRoute();



await app.RunAsync();
