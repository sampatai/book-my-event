
using System.Reflection.Emit;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.OwnsOne(u => u.Address, address =>
        {
            // Optional: configure owned properties
            address.Property(a => a.Street);
            address.Property(a => a.Suburb);
            address.Property(a => a.City);
            address.Property(a => a.State);
            address.Property(a => a.Postcode);
            address.Property(a => a.Country);
        });
        builder.OwnsMany(st => st.RefreshTokens, r =>
        {
            r.WithOwner().HasForeignKey("Id");
            r.Property<long>("Id").UseHiLo("reservationseq");
            r.HasKey("Id");
            r.Property(res => res.Token).IsRequired().HasMaxLength(1000);
            r.Property(res => res.DateCreatedUtc).IsRequired();
            r.Property(res => res.DateExpiresUtc).IsRequired();

            r.ToTable("User_RefreshTokens");
        });

    }
}
