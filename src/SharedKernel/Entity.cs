namespace SharedKernel;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public virtual long Id { get; protected init; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Raise(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool IsTransient() => EqualityComparer<long>.Default.Equals(Id, default);

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return !IsTransient() ? Id.GetHashCode() : GetType().GetHashCode();
    }
}


public abstract class AuditableEntity : Entity
   
{
    public DateTimeOffset CreatedDate { get; private set; } = DateTimeOffset.UtcNow;
    public long CreatedBy { get; private set; } = default!;
    public DateTimeOffset? LastModifiedDate { get; private set; }
    public long LastModifiedBy { get; private set; } = default!;

    public void SetCreationAudits(long createdBy)
    {
        CreatedDate = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetModificationAudits(long modifiedBy)
    {
        LastModifiedDate = DateTimeOffset.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
