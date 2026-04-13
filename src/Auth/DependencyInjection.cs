using Auth.Infrastructure.Data;
using Auth.Infrastructure.Database;
using Domain.Users.Root;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auth
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddDatabaseDeveloperPageExceptionFilter();
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AuthenticationDbContext>(
                options =>
                {
                    options
                    .UseNpgsql(connectionString, npgsqlOptions =>
                        npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default));
                    options.UseSnakeCaseNamingConvention();
                    // This sets the default tracking behavior to NoTracking
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }, ServiceLifetime.Scoped);

            services.AddDataProtection();

            // Register Identity with roles and default UI (provides the login page)
            services.AddIdentity<User, IdentityRole<long>>(options =>
            {
                // Password configuration
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;

                // Require confirmed account for sign-in (useful for SSO flows)
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI(); // Adds the default Razor Pages UI (includes the Login page)


            return services;
        }
    }
   
}
