using Domain.Navigation.Root;
using Domain.Pandit.Root;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class NavigationItemConfiguration : IEntityTypeConfiguration<NavigationItem>
    {
        public void Configure(EntityTypeBuilder<NavigationItem> builder)
        {
          
            builder.ToTable(nameof(NavigationItem)
                 .Pluralize().Underscore());
            // 2. Primary Key
            builder.HasKey(x => x.Id);

            // 3. Properties
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Icon)
                .HasMaxLength(50);

            builder.Property(x => x.RequiredPermission)
                .HasMaxLength(200);

            // 4. Hierarchical Relationship (Self-Referencing)
            builder.HasMany(x => x.Children)
                .WithOne() // No navigation property back to parent in the domain
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. DDD Encapsulation: Backing Field for Children
            // This allows EF to write to the _children list while exposing it as IReadOnlyCollection
            builder.Metadata
                .FindNavigation(nameof(NavigationItem.Children))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 6. Navigation items that are "Roots" (ParentId is null)
            builder.HasIndex(x => x.ParentId);
        }
    }
}
