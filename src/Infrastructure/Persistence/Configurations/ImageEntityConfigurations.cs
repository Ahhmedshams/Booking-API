using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ImageEntityConfigurations : IEntityTypeConfiguration<ImageEntity>
    {
        public void Configure(EntityTypeBuilder<ImageEntity> builder)
        {

            builder.Property(e => e.Uri)
                   .HasColumnType("nvarchar(2048)");

            builder.Property(e => e.Discriminator)
              .HasColumnType("nvarchar(100)");

            builder.ToTable("Images");

            builder.HasDiscriminator<string>("Discriminator")
            .HasValue<ServiceImage>(nameof(ServiceImage))
            .HasValue<ResourceTypeImage>(nameof(ResourceTypeImage))
            .HasValue<ResourceImage>(nameof(ResourceImage));

            // builder.DiscriminatorValue("ImageEntity");

            /*  builder.HasDiscriminator<string>("EntityType")
                 .HasValue<ServiceImage>("Service")
                 .HasValue<ResourceTypeImage>("ResourceType")
                 .HasValue<ResourceImage>("Resource");*/
        }
    }   
}
