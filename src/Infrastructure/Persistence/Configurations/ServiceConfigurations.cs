using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations
{
    public class ServiceConfigurations : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(s => s.Name)
                   .IsRequired();
            builder.HasIndex(s => s.Name)
                .IsUnique();

            builder.Property(s => s.Description)
                   .IsRequired();
            builder.Property(s => s.Status)
                    .HasConversion<string>()
                    .HasDefaultValue(ServiceStatus.Active);

            builder.Property(s => s.CreatedOn)
                   .HasDefaultValueSql("GETDATE()");
            builder.Property(s => s.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
