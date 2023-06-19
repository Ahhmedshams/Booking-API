using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourceTypeImageConfigurations : IEntityTypeConfiguration<ResourceTypeImage>
    {
        public void Configure(EntityTypeBuilder<ResourceTypeImage> builder)
        {
            //builder.HasBaseType<ImageEntity>();

            builder.HasOne(rti => rti.ResourceType)
                   .WithMany(rt => rt.Images)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
