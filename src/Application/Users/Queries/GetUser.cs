using Application.Abstractions.IRepository;

namespace Application.Users.Queries;

public static class GetUser
{
    public sealed record Query(long UserId) : IQuery<UserDto>;

    public sealed class Handler(IUserRepository userRepository) : IQueryHandler<Query, UserDto>
    {
        public Task<Result<UserDto>> Handle(Query query, CancellationToken cancellationToken)
        {
            return userRepository.GetByIdAsync(query.UserId, cancellationToken);
        }
    }
}
