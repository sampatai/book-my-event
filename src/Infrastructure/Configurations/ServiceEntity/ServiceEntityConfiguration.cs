using Domain.ServiceEntity.Root;

namespace Infrastructure.Configurations
{
    public class ServiceEntityConfiguration : IEntityTypeConfiguration<ServiceEntity>
    {
        public void Configure(EntityTypeBuilder<ServiceEntity> builder)
        {
            builder.ToTable(nameof(ServiceEntity).Pluralize().Underscore());

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.StartDate)
                .IsRequired();

            builder.Property(e => e.EndDate);

            builder.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            builder.Property(e => e.IsActive)
                .IsRequired();

            builder.Property(e => e.ContactEmail)
                .HasMaxLength(256);

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(32);

            builder.Property(e => e.WebsiteUrl)
                .HasMaxLength(256);
            builder.Property(e => e.TimeZone)
                .HasMaxLength(50);

            // Value Object: Address (owned type)
            builder.OwnsOne(e => e.Address, address => {
                address.Property(a => a.Street).HasMaxLength(200).HasColumnName("address_street");
                address.Property(a => a.City).HasMaxLength(100).HasColumnName("address_city");
                address.Property(a => a.State).HasMaxLength(100).HasColumnName("address_state");
                address.Property(a => a.PostalCode).HasMaxLength(20).HasColumnName("address_postal_code");
                address.Property(a => a.Country).HasMaxLength(100).HasColumnName("address_country");
            });


        }
    }
}
