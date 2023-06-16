using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ResourceSpecialCharacteristicsConfigurations : IEntityTypeConfiguration<ResourceSpecialCharacteristics>
    {
        public void Configure(EntityTypeBuilder<ResourceSpecialCharacteristics> builder)
        {
            builder.HasKey(e => e.ID);

        }
    }
}
