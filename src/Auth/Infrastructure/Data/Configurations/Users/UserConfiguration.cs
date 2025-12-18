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

        builder.OwnsOne(u => u.Address, address => {
            // Optional: configure owned properties
            address.Property(a => a.Street);
            address.Property(a => a.Suburb);
            address.Property(a => a.City);
            address.Property(a => a.State);
            address.Property(a => a.PostalCode);
            address.Property(a => a.Country);
        });
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(u => u.LastName)
             .IsRequired()
            .HasMaxLength(250);
        builder.Property(u => u.AlternativeContact)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasOne<TenantEntity>() // Assuming ServiceEntity is the related entity
            .WithMany()
            .HasForeignKey(u => u.ServiceEntityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
