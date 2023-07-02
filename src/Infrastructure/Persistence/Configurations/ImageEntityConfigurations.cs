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
            /* builde.Property("Discriminator")
             .HasMaxLength(200);*/

              builder.HasDiscriminator<string>("Discriminator")
               .HasValue<ServiceImage>(nameof(ServiceImage))
               //.HasValue<ResourceTypeImage>(nameof(ResourceTypeImage))
               .HasValue<ResourceImage>(nameof(ResourceImage));

        }
    }   
}
