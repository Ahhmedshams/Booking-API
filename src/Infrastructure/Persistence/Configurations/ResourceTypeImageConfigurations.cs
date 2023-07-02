/*using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
            builder.HasBaseType<ImageEntity>();
            *//* builder.HasOne(si => si.ResourceType)
            .WithMany(s => s.Images)
            .OnDelete(DeleteBehavior.Cascade);

             builder.Property(si => si.Discriminator)
                .HasDefaultValue(nameof(ResourceTypeImage));*//*
        }
    }
}
*/