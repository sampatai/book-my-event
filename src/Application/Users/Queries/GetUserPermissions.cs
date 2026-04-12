using Application.Abstractions.IRepository;

namespace Application.Users.Queries;

public static class GetUserPermissions
{
    public sealed record Query(Guid UserId) : IQuery<IReadOnlyList<string>>;

    public sealed class Handler(IUserRepository userRepository) : IQueryHandler<Query, IReadOnlyList<string>>
    {
        public Task<Result<IReadOnlyList<string>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return userRepository.GetPermissionsAsync(query.UserId, cancellationToken);
        }
    }
}
