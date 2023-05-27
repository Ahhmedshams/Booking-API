using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class ServiceMetadataConfigurations : IEntityTypeConfiguration<ServiceMetadata>
    {
        public void Configure(EntityTypeBuilder<ServiceMetadata> builder)
        {
            builder.HasKey(m => new { m.ServiceId, m.ResourceTypeId });

            //builder.HasOne(m => m.Service)
            //       .WithMany()
            //       .HasForeignKey(m => m.ServiceId);
            
            //builder.HasOne(m => m.ResourceType)
            //    .WithMany()
            //    .HasForeignKey(m => m.ResourceTypeId);

            builder.Property(m => m.CreatedOn)
                  .HasDefaultValueSql("GETDATE()");
            builder.Property(m => m.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
