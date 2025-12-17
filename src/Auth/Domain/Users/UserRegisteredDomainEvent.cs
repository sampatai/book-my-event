using SharedKernel;

namespace Auth.Domain.Users;

public sealed record UserRegisteredDomainEvent(long UserId) : IDomainEvent;
