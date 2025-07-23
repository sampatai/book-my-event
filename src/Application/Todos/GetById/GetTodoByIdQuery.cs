using Application.Abstractions.Messaging;

namespace Application.Todos.GetById;

public sealed record GetTodoByIdQuery(long TodoItemId) : IQuery<TodoResponse>;
