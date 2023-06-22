using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourceImageConfigurations : IEntityTypeConfiguration<ResourceImage>
    {
        public void Configure(EntityTypeBuilder<ResourceImage> builder)
        {
            builder.HasBaseType<ImageEntity>();
            /*builder.HasOne(si => si.Resource)
             .WithMany(s => s.Images)
             .OnDelete(DeleteBehavior.Cascade);

            builder.Property(si => si.Discriminator)
               .HasDefaultValue(nameof(ResourceImage));*/
        }
    }
}
