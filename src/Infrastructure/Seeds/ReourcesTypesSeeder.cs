using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeds
{
    public static class ReourcesTypesSeeder
    {

        public static async Task SeedResoucesTypesSchudaulShownAsync(ApplicationDbContext context)
        {
            var type = new ResourceType() { Name = "Rooms", Shown = true, HasSchedual = true };

            var resourceType = await context.ResourceTypes.Where(r => r.Name == type.Name).FirstOrDefaultAsync();

            if (resourceType == null)
            {
               await context.ResourceTypes.AddAsync(type);
            }

            await SeedMetaDataSchedaulShown(context, type.Name);

        }

        public static async Task SeedResoucesTypesMaterialAsync(ApplicationDbContext context)
        {
            var type = new ResourceType() { Name = "Type 1", Shown = true, HasSchedual = true };

            // TODO
         

        }

        public static async Task SeedResoucesTypesSchedualNotShownAsync(ApplicationDbContext context)
        {
            var type = new ResourceType() { Name = "Type 1", Shown = true, HasSchedual = true };

            // TODO


        }

        private static async Task SeedMetaDataMaterial(ApplicationDbContext context, string resourceTypeName)
        {
            // TODO

        }

        private static async Task SeedMetaDataSchedaulNotShown(ApplicationDbContext context, string resourceTypeName)
        {
            // TODO

        }

        private static async Task SeedMetaDataSchedaulShown(ApplicationDbContext context, string resourceTypeName)
        {
            var resourceType = context.ResourceTypes.FirstOrDefault(r => r.Name == resourceTypeName);

            if (resourceType != null)
            {
                IEnumerable<ResourceMetadata> resourceMetadata = new List<ResourceMetadata>()
                {
                        new ResourceMetadata() { AttributeName = "Attribute 1", AttributeType = "string" , ResourceType = resourceType},
                        new ResourceMetadata() { AttributeName = "Attribute 2", AttributeType = "string" , ResourceType = resourceType },
                        new ResourceMetadata() { AttributeName = "Attribute 3", AttributeType = "bool", ResourceType = resourceType },
                        new ResourceMetadata() { AttributeName = "Attribute 4", AttributeType = "Number", ResourceType = resourceType },
                        new ResourceMetadata() { AttributeName = "Attribute 5", AttributeType = "string", ResourceType = resourceType }
                 };

                IEnumerable<string> attributes = new List<string>() { "Attribute 1", "Attribute 2", "Attribute 4", "Attribute 5" };

                var resourceMetaDataExist = await context.ResourceMetadata.Where(rm => attributes.Contains(rm.AttributeName)).FirstOrDefaultAsync();

                if (resourceMetaDataExist == null)
                    await context.ResourceMetadata.AddRangeAsync(resourceMetadata);
            }
        }

    }
}
