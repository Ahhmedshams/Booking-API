using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ScheduleConfigurations : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            //builder.HasOne<Resource>().WithMany().HasForeignKey(e => e.ResourceId);
            //builder.HasKey(i =>i.ScheduleID);
            builder.Property(x => x.FromDate).HasColumnType("DATE").IsRequired();
            builder.Property(x => x.ToDate).HasColumnType("DATE").IsRequired();
        }
    }
}
