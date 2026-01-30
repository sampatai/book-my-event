using Domain.Common.Enums;
using Domain.Devotee.Entities;
using Domain.Devotee.Root;
using Domain.Pandit.Root;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DevoteeConfiguration : IEntityTypeConfiguration<Devotee>
    {
        public void Configure(EntityTypeBuilder<Devotee> builder)
        {
            builder.ToTable(nameof(Devotee)
                 .Pluralize().Underscore());
            builder.HasKey(x => x.Id);
            builder.Ignore(x => x.DomainEvents);
            builder.Property(d => d.DevoteeId)
           .IsRequired(true);
            builder.HasIndex(d => d.DevoteeId)
                .IsUnique(true);

            builder.Property(d => d.UserId).IsRequired();
            builder.Property(d => d.FullName).IsRequired().HasMaxLength(300);
            builder.Property(d => d.VerificationState)
            .HasConversion(
                v => v.Id,
                id => Enumeration.FromValue<VerificationState>(id))
            .IsRequired();

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

            builder.HasMany(d => d.Verifications)
             .WithOne()
             .HasForeignKey("DevoteeId") // Shadow foreign key
             .OnDelete(DeleteBehavior.Cascade);

        }
    }

    public class DevoteeVerificationConfiguration : IEntityTypeConfiguration<DevoteeVerification>
    {
        public void Configure(EntityTypeBuilder<DevoteeVerification> builder)
        {
            builder.ToTable(nameof(DevoteeVerification)
                  .Pluralize().Underscore());
            builder.HasKey(v => v.Id);
            builder.Property(d => d.VerificationId)
          .IsRequired(true);
            builder.HasIndex(d => d.VerificationId)
                .IsUnique(true);


            builder.Property(v => v.DocumentName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.DocumentPath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.IsVerified)
                .HasDefaultValue(false);
        }
    }
}