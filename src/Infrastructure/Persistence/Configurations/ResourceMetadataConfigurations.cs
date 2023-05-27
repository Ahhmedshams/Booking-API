using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class ResourceMetadataConfigurations : IEntityTypeConfiguration<ResourceMetadata>
    {
        public void Configure(EntityTypeBuilder<ResourceMetadata> builder)
        {
            builder.HasKey(e => e.AttributeId);
            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            builder.HasOne(e => e.ResourceType)
                .WithMany(r=>r.Metadata)
                .HasForeignKey(e => e.ResourceTypeId);
        }
    }
}
