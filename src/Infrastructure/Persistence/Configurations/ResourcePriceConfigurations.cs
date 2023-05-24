using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourcePriceConfigurations : IEntityTypeConfiguration<ResourcePrice>
    {
        public void Configure(EntityTypeBuilder<ResourcePrice> builder)
        {
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

            builder.HasOne(e => e.ResourceType)
                .WithMany()
                .HasForeignKey(e => e.ResourceTypeId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(e => e.ResourceData)
                .WithMany()
                .HasForeignKey(e => e.ResourceId);

            builder.Property(e => e.Price).HasColumnType("decimal(5,2)");

        }
    }
}
