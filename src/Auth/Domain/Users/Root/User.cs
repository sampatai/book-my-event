using Auth.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Auth.Domain.Users.Root;

public class User : IdentityUser<long>, IAggregateRoot
{
    protected User()
    {

    }
    public Guid UserId {  get; private set; }
    public long? ServiceEntityId { get; private set; }


    public User(
        string email,
        string phoneNumber)
    {
      
        UserName = email;
        Email = email;
        PhoneNumber = phoneNumber;
        UserId = Guid.NewGuid();
    }
   
    public void SetServiceEntityId(long serviceEntityId)
    {
        ServiceEntityId = serviceEntityId;
    }
}
