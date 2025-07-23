using SharedKernel;

namespace Domain.Todos;

public static class TodoItemErrors
{
    public static Error AlreadyCompleted(long todoItemId) => Error.Problem(
        "TodoItems.AlreadyCompleted",
        $"The todo item with Id = '{todoItemId}' is already completed.");

    public static Error NotFound(long todoItemId) => Error.NotFound(
        "TodoItems.NotFound",
        $"The to-do item with the Id = '{todoItemId}' was not found");
}
