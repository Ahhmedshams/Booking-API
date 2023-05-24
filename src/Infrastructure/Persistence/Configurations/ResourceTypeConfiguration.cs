using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourceTypeConfigration : IEntityTypeConfiguration<ResourceType>
    {
        public void Configure(EntityTypeBuilder<ResourceType> builder)
        {
            builder.HasKey(x => x.Id);

        }
    }
}