﻿using Application.Abstractions.Messaging;
using Domain.Todos;

namespace Application.Todos.Create;

public sealed class CreateTodoCommand : ICommand<long>
{
    public long UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public Priority Priority { get; set; }
}
