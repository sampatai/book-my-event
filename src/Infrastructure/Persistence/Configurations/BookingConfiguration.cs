using Domain.Booking.Root;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.BookingId);

            builder.Property(b => b.PanditId).IsRequired();
            builder.Property(b => b.DevoteeId).IsRequired();
            builder.Property(b => b.PujaTypeId).IsRequired();
            builder.Property(b => b.BookingDate).IsRequired();
            builder.Property(b => b.BookingTime).IsRequired();
            builder.Property(b => b.SpecialInstructions).HasMaxLength(500);
            builder.Property(b => b.BookingStatus).IsRequired();
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
        }
    }
}