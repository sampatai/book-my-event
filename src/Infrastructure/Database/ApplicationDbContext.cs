using OpenIddict.EntityFrameworkCore.Models;
using Wolverine;
using Wolverine.Runtime;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext

    : IdentityDbContext<User, IdentityRole<long>, long>, IUnitOfWork
{
    private readonly IMessageBus _messageBus;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
    IMessageBus messageBus) : base(options)
    {
        //_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        //_currentUserHelper = currentUserHelper ?? throw new ArgumentNullException(nameof(currentUserHelper));
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        System.Diagnostics.Debug.WriteLine("OrderingContext::ctor ->" + this.GetHashCode());

    }

    public DbSet<OpenIddictEntityFrameworkCoreApplication> OpenIddictApplications =>  Set<OpenIddictEntityFrameworkCoreApplication>();
    public DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIddictAuthorizations => Set<OpenIddictEntityFrameworkCoreAuthorization>();
    public DbSet<OpenIddictEntityFrameworkCoreScope> OpenIddictScopes => Set<OpenIddictEntityFrameworkCoreScope>();
    public DbSet<OpenIddictEntityFrameworkCoreToken> OpenIddictTokens => Set<OpenIddictEntityFrameworkCoreToken>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations in the assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Set the default schema (if needed)
        builder.HasDefaultSchema(Schemas.Default);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken)
    {
        UpdateAuditableEntities();

      
        // 2. Save changes
        await base.SaveChangesAsync(cancellationToken);
        // 1. Dispatch domain events BEFORE save — ensures atomicity
        await PublishDomainEventsAsync();
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

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents.ToList(); // Copy events
                entity.ClearDomainEvents();                // Clear from entity
                return events;
            })
            .ToList();

        foreach (IDomainEvent? domainEvent in domainEvents)
            await _messageBus.PublishAsync(domainEvent);
    }
}
