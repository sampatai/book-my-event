using Application.Abstractions.IRepository;

namespace Application.Users.Commands;

public static class CreateUser
{
    public sealed record Command(string Email, string? PhoneNumber, string Password) : ICommand<long>;

    public sealed class Handler(IUserRepository userRepository) : ICommandHandler<Command, long>
    {
        public Task<Result<long>> Handle(Command command, CancellationToken cancellationToken)
        {
            return userRepository.CreateAsync(command.Email, command.PhoneNumber, command.Password, cancellationToken);
        }
    }
}
