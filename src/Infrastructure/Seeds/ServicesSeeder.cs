using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeds
{
    public static class ServicesSeeder
    {
        public static async Task SeedServicesAsync(ApplicationDbContext context)
        {

            var service = new Service() { Name= "Book room"};

            var existedService = await context.Set<Service>().FirstOrDefaultAsync(s => s.Name == service.Name);
            
            if (existedService == null) { 
            
                context.Set<Service>().Add(service);
            }
        }

        private static async Task SeedServicesMetaDataAsync(IServiceMetadataRepo serviceMetadataRepo )
        {

        }
    }
}
