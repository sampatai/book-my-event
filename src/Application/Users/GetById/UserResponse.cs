namespace Application.Users.GetById;

public sealed record UserResponse
{
    public long Id { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }
}
