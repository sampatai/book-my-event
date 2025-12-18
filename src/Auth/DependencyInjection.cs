using Auth.Domain.Users.Root;
using Auth.Infrastructure.Data;
using Auth.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
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
                    // This sets the default tracking behavior to NoTracking
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }, ServiceLifetime.Scoped);

            services.AddDataProtection();

            services.AddIdentityCore<User>(options =>
            {
                // Password configuration
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // For email confirmation
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<IdentityRole<long>>() // Use IdentityRole<long>
            .AddRoleManager<RoleManager<IdentityRole<long>>>() // Use RoleManager with long as the key type
            .AddEntityFrameworkStores<AuthenticationDbContext>() // Provide our context
            .AddSignInManager<SignInManager<User>>() // Use SignInManager
            .AddUserManager<UserManager<User>>() // Use UserManager to create users
            .AddDefaultTokenProviders();
            //.AddDefaultUI();// Enable token providers for email confirmation
            return services;
        }
    }
   
}
