using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
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
           // builder.HasBaseType<ImageEntity>();

            builder.HasOne(ri => ri.Resource)
                   .WithMany(r => r.Images)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
