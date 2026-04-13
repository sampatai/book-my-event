using Application.Abstractions.IRepository;

namespace Application.Users.Queries;

public static class ListUsers
{
    public sealed record Query : IQuery<IReadOnlyList<UserDto>>;

    public sealed class Handler(IUserRepository userRepository) : IQueryHandler<Query, IReadOnlyList<UserDto>>
    {
        public Task<Result<IReadOnlyList<UserDto>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return userRepository.ListAsync(cancellationToken);
        }
    }
}
