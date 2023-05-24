using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourceScheduleConfiguration : IEntityTypeConfiguration<ResourceSchedule>
    {
        public void Configure(EntityTypeBuilder<ResourceSchedule> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

            builder.Property(x => x.StartTime).HasColumnType("time").IsRequired();
            builder.Property(x => x.EndTime).HasColumnType("time").IsRequired();
            builder.Property(x => x.StartDate).HasColumnType("DATE").IsRequired();
            builder.Property(x => x.EndDate).HasColumnType("DATE").IsRequired();

            builder.Property(e => e.StartTime).HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

            builder.Property(e => e.EndTime).HasConversion(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

            builder.HasOne(e => e.ResourceData)
                .WithMany()
                .HasForeignKey(e => e.ResourceId);



        }
    }
}
