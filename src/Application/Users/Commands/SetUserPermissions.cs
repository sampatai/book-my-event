using Application.Abstractions.IRepository;

namespace Application.Users.Commands;

public static class SetUserPermissions
{
    public sealed record Command(long UserId, IReadOnlyCollection<string> Permissions) : ICommand;

    public sealed class Handler(IUserRepository userRepository) : ICommandHandler<Command>
    {
        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            return userRepository.SetPermissionsAsync(command.UserId, command.Permissions, cancellationToken);
        }
    }
}
