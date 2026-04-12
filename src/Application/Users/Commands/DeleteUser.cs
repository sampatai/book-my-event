using Application.Abstractions.IRepository;

namespace Application.Users.Commands;

public static class DeleteUser
{
    public sealed record Command(long UserId) : ICommand;

    public sealed class Handler(IUserRepository userRepository) : ICommandHandler<Command>
    {
        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            return userRepository.DeleteAsync(command.UserId, cancellationToken);
        }
    }
}
