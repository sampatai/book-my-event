using Application.Users;

namespace Application.Abstractions.IRepository;

public interface IUserRepository
{
    Task<Result<long>> CreateAsync(string email, string? phoneNumber, string password, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(long userId, string email, string? phoneNumber, long? serviceEntityId, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(long userId, CancellationToken cancellationToken);

    Task<Result<UserDto>> GetByIdAsync(long userId, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<UserDto>>> ListAsync(CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<UserClaimDto>>> GetClaimsAsync(long userId, CancellationToken cancellationToken);

    Task<Result> SetClaimsAsync(long userId, IReadOnlyCollection<UserClaimDto> claims, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<string>>> GetPermissionsAsync(long userId, CancellationToken cancellationToken);

    Task<Result> SetPermissionsAsync(long userId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken);
}
