using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ServiceImageConfigurations : IEntityTypeConfiguration<ServiceImage>
    {
        public void Configure(EntityTypeBuilder<ServiceImage> builder)
        {

            builder.HasBaseType<ImageEntity>();
            /* builder.HasOne(si => si.Service)
             .WithMany(s => s.Images)
             .OnDelete(DeleteBehavior.Cascade);

             builder.Property(si => si.Discriminator)
                .HasDefaultValue(nameof(ServiceImage));
 */
            // builder.Property("Discriminator").HasValue(nameof(ServiceImage));
        }
    }
}
