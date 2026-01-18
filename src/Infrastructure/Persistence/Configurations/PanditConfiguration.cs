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

            builder.HasOne(x => x.VerificationState)
            .WithMany()
            .HasForeignKey("VerificationStateId")
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}