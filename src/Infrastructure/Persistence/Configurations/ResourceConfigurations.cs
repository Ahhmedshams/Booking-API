using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourceConfigurations : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
            
            builder.HasOne(e => e.ResourceType)
                .WithMany()
                .HasForeignKey(e => e.ResourceTypeId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Property(e => e.Price).HasColumnType("decimal(5,2)");
            builder.HasQueryFilter(r => r.IsDeleted == false);


            // builder.HasMany<Schedule>().WithOne().HasForeignKey(e=>e.ScheduleID);
        }
    }
}
