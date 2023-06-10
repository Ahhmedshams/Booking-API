using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ScheduleItemConfiguration : IEntityTypeConfiguration<ScheduleItem>
    {
        public void Configure(EntityTypeBuilder<ScheduleItem> builder)
        {
            builder.HasKey(i => new { i.Day, i.StartTime, i.EndTime });

            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

            builder.Property(x => x.StartTime).HasColumnType("TIME").IsRequired();
            builder.Property(x => x.EndTime).HasColumnType("TIME").IsRequired();

            builder.Property(e => e.StartTime).HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

            builder.Property(e => e.EndTime).HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

            //builder.HasOne<Schedule>().WithMany().HasForeignKey(e => e.ScheduleId);
            builder.Property(i => i.Available)
                   .HasDefaultValue(true);

            builder.Property(i => i.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
