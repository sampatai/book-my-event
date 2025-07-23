using SharedKernel;

namespace Domain.Users;

public sealed record UserRegisteredDomainEvent(long UserId) : IDomainEvent;
