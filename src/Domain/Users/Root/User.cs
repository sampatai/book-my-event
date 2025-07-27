using Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Domain.Users.Root;

public class User : IdentityUser<long>, IAggregateRoot
{
    protected User()
    {

    }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string AlternativeContact { get; private set; }
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;
    public Address Address { get; private set; }
    public void SetRefereshToken(string token, DateTime expireDate)
    {
        _refreshTokens.Clear();
        _refreshTokens.Add(new RefreshToken(token, expireDate));
    }
    public User(string firstname,
        string lastname,
        string alternativeContact,
        string username,
        string email,
        string phoneNumber)
    {
        FirstName = firstname;
        LastName = lastname;
        AlternativeContact = alternativeContact;
        UserName = username;
        Email = email;
        PhoneNumber = phoneNumber;
    }
    public void SetAddress(string street, string suburb,
        string state, string postcode, string country, string city)
    {
        Address = new(street, suburb, state, postcode, country, city);
    }
}
