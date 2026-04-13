using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Domain.Users.Root;

public class User : IdentityUser<long>, IAggregateRoot
{
    protected User()
    {
    }

    public Guid UserId { get; private set; }

    public User(
        string email,
        string phoneNumber)
    {
        UserName = email;
        Email = email;
        PhoneNumber = phoneNumber;
        UserId = Guid.NewGuid();
    }

    
}
