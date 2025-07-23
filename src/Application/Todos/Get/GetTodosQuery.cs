using Application.Abstractions.Messaging;

namespace Application.Todos.Get;

public sealed record GetTodosQuery(long UserId) : IQuery<List<TodoResponse>>;
