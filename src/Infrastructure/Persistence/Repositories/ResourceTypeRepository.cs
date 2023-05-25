using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceTypeRepository : CRUDRepository<ResourceType>
    {
        public ResourceTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
