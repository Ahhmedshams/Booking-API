using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceDataRepository : CRUDRepository<ResourceData>, IResourceDataRepo
    {
        public ResourceDataRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ResourceData> AddRange(IEnumerable<ResourceData> entities)
        {
            _context.ResourceData.AddRange(entities);
            _context.SaveChanges();
            return entities;
        }
      

        public async Task<IEnumerable<ResourceData>> AddRangeAsync(IEnumerable<ResourceData> entities)
        {
            await _context.ResourceData.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public IEnumerable<ResourceData> Find(Expression<Func<ResourceData, bool>> predicate)
        {
            return _context.ResourceData.Where(predicate).ToList();
        }
    }
}
