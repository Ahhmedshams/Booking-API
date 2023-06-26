using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourceSpecialCharacteristicsConfigurations : IEntityTypeConfiguration<ResourceSpecialCharacteristics>
    {
        public void Configure(EntityTypeBuilder<ResourceSpecialCharacteristics> builder)
        {
            builder.HasKey(e => e.ID);

        }

    }
    public class ScheduleItemConfigurations : IEntityTypeConfiguration<ScheduleItem>
    {
        public void Configure(EntityTypeBuilder<ScheduleItem> builder)
        {
            builder.HasOne(si => si.ResourceSpecialCharacteristics)
                .WithOne(rs => rs.ScheduleItem)
                .HasForeignKey<ResourceSpecialCharacteristics>(rs => rs.ScheduleID);
        }
    }
}
