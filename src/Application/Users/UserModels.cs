namespace Application.Users;

public sealed record UserDto(
    long Id,
    Guid UserId,
    string? Email,
    string? UserName,
    string? PhoneNumber);

public sealed record UserClaimDto(string Type, string Value);

public static class UserManagementErrors
{
    public static Error NotFound(long userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with id '{userId}' was not found.");

    public static Error InvalidInput(string message) => Error.Failure("Users.InvalidInput", message);

    public static Error OperationFailed(string message) => Error.Failure("Users.OperationFailed", message);
}
