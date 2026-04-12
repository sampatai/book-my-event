using Application.Abstractions.IRepository;

namespace Application.Users.Commands;

public static class SetUserClaims
{
    public sealed record Command(long UserId, IReadOnlyCollection<UserClaimDto> Claims) : ICommand;

    public sealed class Handler(IUserRepository userRepository) : ICommandHandler<Command>
    {
        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            return userRepository.SetClaimsAsync(command.UserId, command.Claims, cancellationToken);
        }
    }
}
