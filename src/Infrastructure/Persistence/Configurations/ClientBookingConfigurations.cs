using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ClientBookingConfigurations : IEntityTypeConfiguration<ClientBooking>
    {
        public void Configure(EntityTypeBuilder<ClientBooking> builder)
        {
            builder.Property(c => c.Date)
                   .HasColumnType("Date");
            builder.Property(c => c.Time)
                   .HasColumnType("Time");
            builder.Property(c => c.Duration)
                   .HasColumnType("Time");
            builder.Property(c => c.Status)
                    .HasConversion<string>()
                    .HasDefaultValue(BookingStatus.Pending);

            builder.HasOne(c => c.Service)
                .WithMany()
                .HasForeignKey(c => c.ServiceId);

            builder.Property(c => c.CreatedOn)
                   .HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
