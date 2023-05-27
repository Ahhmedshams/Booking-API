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
            builder.HasKey(i => new { i.ScheduleID, i.ResourceId });
            builder.Property(x => x.FromDate).HasColumnType("DATE").IsRequired();
            builder.Property(x => x.ToDate).HasColumnType("DATE").IsRequired();
        }
    }
}
