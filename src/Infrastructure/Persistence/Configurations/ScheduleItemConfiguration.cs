using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ScheduleItemConfiguration : IEntityTypeConfiguration<ScheduleItem>
    {
        public void Configure(EntityTypeBuilder<ScheduleItem> builder)
        {
            builder.HasKey(i => new { i.ScheduleID, i.Day,i.StartTime,i.EndTime });

            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

           

            builder.Property(x => x.StartTime).HasColumnType("time").IsRequired();
            builder.Property(x => x.EndTime).HasColumnType("time").IsRequired();
           
            builder.Property(e => e.StartTime).HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

            builder.Property(e => e.EndTime).HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

            builder.Property(i => i.CreatedOn)
                 .HasDefaultValueSql("GETDATE()");
            builder.Property(i => i.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
