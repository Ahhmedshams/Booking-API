namespace Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading.Tasks;

    public class ResourceRegionConfiguraion : IEntityTypeConfiguration<ResourceRegion>
    {
        public void Configure(EntityTypeBuilder<ResourceRegion> builder)
        {

            builder.HasKey(x =>new { x.ResourceId, x.RegionId});

            builder.HasOne(rr => rr.Resource)
                  .WithOne(r => r.ResourceRegion)
                  .IsRequired();

            builder.HasOne(rr => rr.Region)
                .WithMany(r => r.ResourceRegions)
                .HasForeignKey(rr => rr.RegionId)
                .IsRequired();


        }
    }
}
