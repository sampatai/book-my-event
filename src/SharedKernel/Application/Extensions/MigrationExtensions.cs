using Microsoft.EntityFrameworkCore;

namespace Web.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations<TDbContext>(this IApplicationBuilder app) where TDbContext : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        dbContext.Database.Migrate();
    }
}
