
using Domain.Pandit.Root;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IUnitOfWork, IApplicationDbContext
{
    public ApplicationDbContext()
    {
        
    }
    public DbSet<Pandit> Pandits { get; set; }

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public  async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return true;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents);
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

            if (entry.State == EntityState.Added){
                entity.SetCreationAudits(0); // Replace 0 with actual userId
            else
                entity.SetModificationAudits(0); // Replace 0 with actual userId

        }
    }


}

    
}