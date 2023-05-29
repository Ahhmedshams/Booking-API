using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<ResourceType>  ResourceTypes { get; set; }
        public DbSet<ResourceMetadata> ResourceMetadata { get; set; }
        public DbSet<Resource> Resource { get; set; }
        public DbSet<ResourceData> ResourceData { get; set; }

        DbSet<Service> Services { get; set; }
        DbSet<ServiceMetadata> ServiceMetadata { get; set; }
        DbSet<BookingItem> BookingItems { get; set; }
        DbSet<ClientBooking> ClientBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
         
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
