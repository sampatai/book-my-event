using Domain.Booking.Enums;
using Domain.Booking.Root;
using Domain.Devotee.Root;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable(nameof(Booking)
                .Pluralize().Underscore());
            builder.HasKey(b => b.Id);
            builder.Property(d => d.BookingId)
                           .IsRequired(true);
            builder.HasIndex(d => d.BookingId)
                .IsUnique(true);
            builder.Property(b => b.PanditId).IsRequired();
            builder.Property(b => b.DevoteeId).IsRequired();
            builder.Property(b => b.PujaTypeId).IsRequired();
            builder.Property(b => b.BookingDate).IsRequired();
            builder.Property(b => b.BookingTime).IsRequired();
            builder.Property(b => b.SpecialInstructions).HasMaxLength(500);

            builder.Property(d => d.BookingStatus)
           .HasConversion(
               v => v.Id,
               id => Enumeration.FromValue<BookingStatus>(id))
           .IsRequired();
            builder.Property(b => b.CancellationReason).HasMaxLength(500);

            builder.OwnsOne(b => b.PujaVenue, a =>
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

            // Ignore Domain Events for EF persistence
            builder.Ignore(b => b.DomainEvents);

            // Indexing for faster lookups
            builder.HasIndex(b => b.PanditId);
            builder.HasIndex(b => b.DevoteeId);
            builder.HasIndex(b => b.BookingDate);
        }
    }
}