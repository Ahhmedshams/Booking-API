using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class ClientBookingConfigurations : IEntityTypeConfiguration<ClientBooking>
    {
        public void Configure(EntityTypeBuilder<ClientBooking> builder)
        {
            builder.Property(c => c.Date)
                   .HasColumnType("Date");
            builder.Property(c => c.StartTime)
                   .HasColumnType("Time");
            builder.Property(c => c.EndTime)
                   .HasColumnType("Time");
            builder.Property(c => c.Status)
                    .HasConversion<string>()
                    .HasDefaultValue(BookingStatus.Pending);
            

            builder.HasOne(c => c.Service)
                .WithMany()
                .HasForeignKey(c => c.ServiceId);

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            builder.Property(c => c.CreatedOn)
                   .HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
