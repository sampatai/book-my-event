using Application.Abstractions.IRepository;

namespace Application.Users.Commands;

public static class UpdateUser
{
    public sealed record Command(long UserId, string Email, string? PhoneNumber, long? ServiceEntityId) : ICommand;

    public sealed class Handler(IUserRepository userRepository) : ICommandHandler<Command>
    {
        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            return userRepository.UpdateAsync(command.UserId, command.Email, command.PhoneNumber, command.ServiceEntityId, cancellationToken);
        }
    }
}
