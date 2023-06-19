using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeds
{
    public static class ReourcesSeeder
    {
        public static async Task SeedResourcesAsync(ApplicationDbContext context)
        {
            var resourceTypeShown = await context.ResourceTypes.FirstOrDefaultAsync(r => r.Shown == true);

            if (resourceTypeShown != null)
            {
                var room1Resource = new Resource() { Name = "room1", Price = 200, ResourceType = resourceTypeShown };
                await context.Resource.AddAsync(room1Resource);
            }

        }

        private static async Task SeedSchedualsShown(ApplicationDbContext context,Resource resource)
        {

        }

        private static async Task SeedSchedualsNotShown(ApplicationDbContext context , Resource resource)
        {

        }



    }
}
