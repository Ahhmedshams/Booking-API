﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.ToTable("Users", "security");

        }
    }
}
