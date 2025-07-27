
namespace Domain.Users.ValueObjects;

public class RefreshToken : ValueObject
{
    protected RefreshToken()
    {

    }
    internal RefreshToken(string token, DateTime dateExpiresUtc)
    {
        Token = Guard.Against.NullOrEmpty(token);
        DateExpiresUtc = Guard.Against.NullOrOutOfSQLDateRange(dateExpiresUtc);
        DateCreatedUtc = DateTime.UtcNow;
    }
    public string Token { get; private set; }
    public DateTime DateCreatedUtc { get; private set; }
    public DateTime DateExpiresUtc { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= DateExpiresUtc;
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Token;
        yield return DateCreatedUtc;
        yield return DateExpiresUtc;
        yield return IsExpired;
    }
}

