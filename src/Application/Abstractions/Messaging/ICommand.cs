namespace Application.Abstractions.Messaging;

// Marker for Commands
public interface ICommand { }

public interface ICommand<out TResponse> : ICommand { }

/// <summary>
/// Defines a contract for handling commands asynchronously.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Handle(TCommand command, CancellationToken cancellationToken);
}
public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}
