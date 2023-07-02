using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class ResourceDataConfigurations : IEntityTypeConfiguration<ResourceData>
    {
        public void Configure(EntityTypeBuilder<ResourceData> builder)
        {
            builder.HasKey(e => new { e.ResourceId, e.AttributeId });
            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

            builder.HasOne(e => e.ResourceMetadata)
                .WithMany()
                .HasForeignKey(e => e.AttributeId);

            builder.HasOne(e => e.Resource)
                .WithMany()
                .HasForeignKey(e => e.ResourceId);


        }
    }
}
