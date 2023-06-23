using Domain.Common;
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
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<ScheduleItem> ScheduleItem { get; set; }
        public DbSet<ResourceReview> ResourceReview { get; set; }
        public DbSet<FAQ> FAQ { get; set; }
        public DbSet<FAQCategory> FAQCategory { get; set; }

        public DbSet<PaymentTransaction> paymentTransactions { get; set; }
        public DbSet<ImageEntity> Images { get; set; }
        public DbSet<ServiceImage> ServiceImages { get; set; }
        public DbSet<ResourceTypeImage> ResourceTypeImages { get; set; }
        public DbSet<ResourceImage> ResourceImages { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }    
       
        public DbSet<Region> Regions { get; set; }

        DbSet<Service> Services { get; set; }
        DbSet<ServiceMetadata> ServiceMetadata { get; set; }
        DbSet<BookingItem> BookingItems { get; set; }
        DbSet<ClientBooking> ClientBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
         
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var Now = DateTime.UtcNow;

            foreach (var entry in base.ChangeTracker.Entries())
            {
                if(entry.Entity is IHasUpdatedOn UpdateEntity)
                {
                    switch(entry.State)
                    {
                        case EntityState.Modified:
                            UpdateEntity.LastUpdatedOn = Now;
                            break;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
