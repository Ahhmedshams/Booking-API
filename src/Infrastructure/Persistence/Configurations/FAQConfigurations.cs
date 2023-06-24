using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class FAQConfigurations : IEntityTypeConfiguration<FAQ>
    {
        public void Configure(EntityTypeBuilder<FAQ> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(m => m.CreatedOn)
                  .HasDefaultValueSql("GETDATE()");
            builder.Property(m => m.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
