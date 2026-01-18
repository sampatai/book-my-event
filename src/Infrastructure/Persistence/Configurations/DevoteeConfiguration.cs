using Domain.Devotee.Root;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DevoteeConfiguration : IEntityTypeConfiguration<Devotee>
    {
        public void Configure(EntityTypeBuilder<Devotee> builder)
        {
            builder.HasKey(d => d.DevoteeId);

            builder.Property(d => d.UserId).IsRequired();
            builder.Property(d => d.FullName).IsRequired().HasMaxLength(200);
            builder.Property(d => d.VerificationState).IsRequired();

            builder.OwnsOne(d => d.Address, a =>
            {
                a.Property(p => p.Street).HasMaxLength(200);
                a.Property(p => p.State).HasMaxLength(100);
                a.Property(p => p.PostalCode).HasMaxLength(20);
                a.Property(p => p.Country).HasMaxLength(100);
                a.Property(p => p.City).HasMaxLength(100);
                a.Property(p => p.AddressLine1).HasMaxLength(200);
                a.Property(p => p.AddressLine2).HasMaxLength(200);
                a.Property(p => p.Timezone).HasMaxLength(100);
            });

           
        }
    }
}