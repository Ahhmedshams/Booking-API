using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ServiceMetadaRepository : CRUDRepositoryAsync<ServiceMetadata>, IServiceMetadataRepo
    {
        public ServiceMetadaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ServiceMetadata> DeleteServiceMDAsyn(int serviceId, int resId)
        {
            var foundEntity = await _context.Set<ServiceMetadata>()
                    .FindAsync(serviceId, resId);

            if (foundEntity == null)
                return null;

            _context.Set<ServiceMetadata>().Remove(foundEntity);
            await _context.SaveChangesAsync();

            return foundEntity;
        }

        public async Task<ServiceMetadata> EditServiceMDAsyn(int serviceId, int resId, ServiceMetadata entity)
        {
            var foundEntity = await _context.Set<ServiceMetadata>()
                .FindAsync(serviceId, resId);

            if (foundEntity == null)
                return null;

            _context.Entry(foundEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return foundEntity;
        }

        public async Task<ServiceMetadata> GetServiceMDByIdAsync(int serviceId, int resId, params Expression<Func<ServiceMetadata, object>>[] includes)
        {
            var query = _context.Set<ServiceMetadata>().AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);

            }
            return await _context.Set<ServiceMetadata>().FindAsync(serviceId, resId);
        }
    }
       
}
