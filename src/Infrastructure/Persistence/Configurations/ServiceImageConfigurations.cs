using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
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
            //builder.HasBaseType<ImageEntity>();

            builder.HasOne(si => si.Service)
                   .WithMany(s => s.Images)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
