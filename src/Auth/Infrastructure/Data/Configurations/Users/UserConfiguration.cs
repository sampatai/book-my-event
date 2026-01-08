using System.Reflection.Emit;
using Auth.Domain.TenantEntity.Root;
using Auth.Domain.Users.Root;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Auth.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.UserId)
            .IsRequired();
        builder.HasIndex(u => u.UserId)
       .IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

       
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(u => u.LastName)
             .IsRequired()
            .HasMaxLength(250);
       

        builder.HasOne<TenantEntity>() // Assuming ServiceEntity is the related entity
            .WithMany()
            .HasForeignKey(u => u.ServiceEntityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
