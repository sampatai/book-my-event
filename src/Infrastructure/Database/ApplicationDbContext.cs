using Application.Abstractions.Data;
using Domain.Todos;
using Domain.Users;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }



    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList(); // Explicitly convert IReadOnlyCollection to List
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken)
    {
        IEnumerable<EntityEntry> entries = ChangeTracker
           .Entries()
           .Where(e => e.Entity is AuditableEntity && (
                   e.State == EntityState.Added
                   || e.State == EntityState.Modified));

        foreach (EntityEntry entityEntry in entries)
        {
            var entity = (AuditableEntity)entityEntry.Entity;
            // var currentUser = (await _currentUserHelper.GetCurrentUser()).UserId; // Replace with actual user context

            if (entityEntry.State == EntityState.Added)
            {
                entity.SetCreationAudits(0);
            }
            else
            {
                entity.SetModificationAudits(0);
            }
        }

        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail
        await PublishDomainEventsAsync();

        await base.SaveChangesAsync(cancellationToken);

        return true;
    }
}
