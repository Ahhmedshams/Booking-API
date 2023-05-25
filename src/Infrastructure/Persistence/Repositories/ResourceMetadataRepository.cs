using Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceMetadataRepository : CRUDRepository<ResourceMetadata>, IResourceMetadataRepo
    {

        public ResourceMetadataRepository(ApplicationDbContext context) :base(context) { }

        public IEnumerable<ResourceMetadata> AddRange(IEnumerable<ResourceMetadata> entities)
        {
             _context.ResourceMetadata.AddRange(entities);
             _context.SaveChanges();
            return entities;
        }

        public async Task<IEnumerable<ResourceMetadata>> AddRangeAsync(IEnumerable<ResourceMetadata> entities)
        {
            await _context.ResourceMetadata.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public void DeleteBulk(Expression<Func<ResourceMetadata, bool>> predicate)
        {
             _context.ResourceMetadata.Where(predicate).ExecuteDelete();
             _context.SaveChanges();
        }

        public async Task DeleteBulkAsync(Expression<Func<ResourceMetadata, bool>> predicate)
        {
            await _context.ResourceMetadata.Where(predicate).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public IEnumerable<ResourceMetadata> Find(Expression<Func<ResourceMetadata, bool>> predicate)
        {
           var res= _context.ResourceMetadata.Where(predicate).ToList();
            return res;
        }
    }
}
