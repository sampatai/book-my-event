using Auth.Domain.TenantEntity.Root;
using Auth.Domain.Users.Root;
using Auth.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel;
namespace Auth.Infrastructure.Data
{
    public class AuthenticationDbContext :  IdentityDbContext<User, IdentityRole<long>, long>, IUnitOfWork
    {
       

        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {
            System.Diagnostics.Debug.WriteLine("OrderingContext::ctor ->");

        }
        public DbSet<TenantEntity> TenantEntities => Set<TenantEntity>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // This line is missing! It adds the OpenIddict tables to EF Core
            builder.UseOpenIddict();
            // Apply all configurations in the assembly
            builder.ApplyConfigurationsFromAssembly(typeof(AuthenticationDbContext).Assembly);

            // Set the default schema (if needed)
            builder.HasDefaultSchema(Schemas.Default);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken)
        {
            UpdateAuditableEntities();


            // 2. Save changes
            await base.SaveChangesAsync(cancellationToken);
            // 1. Dispatch domain events BEFORE save — ensures atomicity
           
            return true;
        }

        private void UpdateAuditableEntities()
        {
            IEnumerable<EntityEntry> entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is AuditableEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (EntityEntry entry in entries)
            {
                var entity = (AuditableEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                    entity.SetCreationAudits(0); // Replace 0 with actual userId
                else
                    entity.SetModificationAudits(0); // Replace 0 with actual userId
            }
        }

        

    }
}
