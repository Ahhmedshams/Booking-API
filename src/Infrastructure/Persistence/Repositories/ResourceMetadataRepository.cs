using Application.Common.Interfaces.Repositories;
using Domain.Entities;
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





        public IEnumerable<ResourceMetadata> Find(Expression<Func<ResourceMetadata, bool>> predicate)
        {
           var res= _context.ResourceMetadata.Where(predicate).ToList();
            return res;
        }

        public async Task<IEnumerable<ResourceMetadata>> FindAsync(Expression<Func<ResourceMetadata, bool>> predicate)
        {
            var res = await _context.ResourceMetadata.Where(predicate).ToListAsync();
            return res;
        }


        public async Task<bool> SoftDeleteAsync(int id)
        {
            var ResoureMetaData = _context.ResourceMetadata.FirstOrDefault(ResMeta => ResMeta.AttributeId == id);

            if (ResoureMetaData == null)
                return false;

            ResoureMetaData.IsDeleted = true;

            _context.ResourceData.Where(res => res.AttributeId ==id).ToList().ForEach(resData =>
               resData.IsDeleted = true
               );

           await _context.SaveChangesAsync();

            return true;
        }

    }
}
