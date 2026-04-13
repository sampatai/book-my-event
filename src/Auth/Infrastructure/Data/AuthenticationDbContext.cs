using Auth.Infrastructure.Database;
using Domain.Users.Root;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OpenIddict.EntityFrameworkCore;
using SharedKernel;
namespace Auth.Infrastructure.Data
{
    public class AuthenticationDbContext :  IdentityDbContext<User, IdentityRole<long>, long>
    {
       

        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {
            System.Diagnostics.Debug.WriteLine("OrderingContext::ctor ->");

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.UseOpenIddict();
            
         
        }

       

        

    }
}
