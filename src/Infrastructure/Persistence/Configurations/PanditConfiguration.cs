using Domain.Common.Enums;
using Domain.Common.ValueObjects;
using Domain.Pandit.Entities;
using Domain.Pandit.Root;
using Domain.ValueObjects;


namespace Infrastructure.Persistence.Configurations
{
    public class PanditConfiguration : IEntityTypeConfiguration<Pandit>
    {
        public void Configure(EntityTypeBuilder<Pandit> builder)
        {
            builder.ToTable(nameof(Pandit)
                .Pluralize().Underscore());
            builder.HasKey(x => x.Id);
            builder.Ignore(x => x.DomainEvents);
            builder.Property(d => d.PanditId)
           .IsRequired(true);
            builder.HasIndex(d => d.PanditId)
                .IsUnique(true);

            builder.Property(p => p.UserId)
                .IsRequired();
            builder.HasIndex(p => p.UserId);

            builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Languages).IsRequired().HasMaxLength(200);
            builder.Property(p => p.ExperienceInYears).IsRequired();
            
            builder.Property(p => p.AverageRating);

            builder.OwnsOne(p => p.Address, a =>
            {
                a.Property(p => p.Street).HasMaxLength(200);
                a.Property(p => p.State).HasMaxLength(100);
                a.Property(p => p.PostalCode).HasMaxLength(20);
                a.Property(p => p.Country).HasMaxLength(100);
                a.Property(p => p.City).HasMaxLength(100);
                a.Property(p => p.AddressLine1).HasMaxLength(200);
                a.Property(p => p.AddressLine2).HasMaxLength(200);
                a.Property(p => p.Timezone).HasMaxLength(100);
                a.WithOwner();
            });

            builder.Property(p => p.VerificationState)
             .HasConversion(
                 v => v.Id,
                 id => Enumeration.FromValue<VerificationState>(id))
             .IsRequired();

            builder.HasMany(p => p.Verifications)
            .WithOne()
            .HasForeignKey("PanditId")
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PujaTypes)
            .WithOne()
            .HasForeignKey("PanditId")
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reviews)
            .WithOne()
            .HasForeignKey("PanditId")
            .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PanditVerificationConfiguration : IEntityTypeConfiguration<PanditVerification>
    {
        public void Configure(EntityTypeBuilder<PanditVerification> builder)
        {
            builder.ToTable(nameof(PanditVerification)
                .Pluralize().Underscore());

            builder.Property(d => d.VerificationId)
           .IsRequired(true);
            builder.HasIndex(d => d.VerificationId)
                .IsUnique(true);
            builder.HasKey(v => v.Id);
            builder.Property(v => v.DocumentName).IsRequired().HasMaxLength(200);
            builder.Property(v => v.DocumentPath).IsRequired().HasMaxLength(500);
        }
    }
    public class PujaTypeConfiguration : IEntityTypeConfiguration<PujaType>
    {
        public void Configure(EntityTypeBuilder<PujaType> builder)
        {
            builder.ToTable(nameof(PujaType)
                .Pluralize().Underscore());

            builder.Property(d => d.PujaTypeId)
           .IsRequired(true);
            builder.HasIndex(d => d.PujaTypeId)
                .IsUnique(true);
            builder.HasKey(pt => pt.Id);
            builder.Property(pt => pt.Name).IsRequired().HasMaxLength(100);
            builder.Property(pt => pt.Description).HasMaxLength(500);
        }
    }

    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable(nameof(Review)
                 .Pluralize().Underscore());

            builder.Property(d => d.ReviewId)
          .IsRequired(true);
            builder.HasIndex(d => d.ReviewId)
                .IsUnique(true);

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Comment).HasMaxLength(4000);
            builder.Property(r => r.Rating).IsRequired();
        }
    }
}