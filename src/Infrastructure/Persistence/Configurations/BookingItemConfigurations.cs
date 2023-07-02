using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingItemConfigurations : IEntityTypeConfiguration<BookingItem>
    {
        public void Configure(EntityTypeBuilder<BookingItem> builder)
        {
            builder.HasKey(i => new { i.BookingId, i.ResourceId });
            builder.Property(i => i.Price).HasColumnType("decimal(18,2)");

            builder.Property(i => i.CreatedOn)
                  .HasDefaultValueSql("GETDATE()");
            builder.Property(i => i.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
