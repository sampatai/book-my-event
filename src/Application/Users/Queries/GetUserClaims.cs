using Application.Abstractions.IRepository;

namespace Application.Users.Queries;

public static class GetUserClaims
{
    public sealed record Query(long UserId) : IQuery<IReadOnlyList<UserClaimDto>>;

    public sealed class Handler(IUserRepository userRepository) : IQueryHandler<Query, IReadOnlyList<UserClaimDto>>
    {
        public Task<Result<IReadOnlyList<UserClaimDto>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return userRepository.GetClaimsAsync(query.UserId, cancellationToken);
        }
    }
}
