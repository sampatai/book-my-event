using SharedKernel;

namespace Domain.Todos;

public sealed record TodoItemDeletedDomainEvent(long TodoItemId) : IDomainEvent;
