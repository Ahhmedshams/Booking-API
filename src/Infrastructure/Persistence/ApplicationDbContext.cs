using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) { }

        DbSet<ResourceType>  ResourceTypes { get; set; }
        DbSet<ResourceMetadata> ResourceMetadata { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
         
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
