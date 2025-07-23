using Application.Abstractions.Messaging;

namespace Application.Todos.Update;

public sealed record UpdateTodoCommand(
    long TodoItemId,
    string Description) : ICommand;
