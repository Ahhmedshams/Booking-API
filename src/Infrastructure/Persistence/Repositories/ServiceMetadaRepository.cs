
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class ServiceMetadaRepository : CRUDRepositoryAsync<ServiceMetadata>, IServiceMetadataRepo
    {
        public ServiceMetadaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ServiceMetadata>> AddBulk(IEnumerable<ServiceMetadata> serviceMetadata)
        {
            await _context.Set<ServiceMetadata>().AddRangeAsync(serviceMetadata);
            await _context.SaveChangesAsync();
            return serviceMetadata;
        }

        public async Task<bool> CheckDuplicateKey(int serviceId, int resTypeId)
        {
            var objectExist = await GetServiceMDByIdAsync(serviceId, resTypeId);
            if (objectExist == null)
                return false;
            return true;
        }

        public async Task DeleteBulk(int serviceId)
        {
            await _context.Set<ServiceMetadata>().Where(s => s.ServiceId == serviceId).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
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
            var foundEntity = await GetServiceMDByIdAsync(serviceId, resId);

            if (foundEntity == null)
                return null;

            await DeleteServiceMDAsyn(serviceId, resId);
            await AddAsync(entity);

            return foundEntity;
        }

        public async Task<IEnumerable<ServiceMetadata>> GetByResourceId(int resId, params Expression<Func<ServiceMetadata, object>>[] includes)
        {
            var servicesMetadata = await _context.Set<ServiceMetadata>()
                                    .Where(s => s.ResourceTypeId == resId)
                                    .ToListAsync();
            if (servicesMetadata.Count() == 0)
                return null;
            return servicesMetadata;
        }

        public async Task<IEnumerable<ServiceMetadata>> GetByServiceId(int serviceId, params Expression<Func<ServiceMetadata, object>>[] includes)
        {
            var servicesMetadata  = await _context.Set<ServiceMetadata>()
                                    .Where(s => s.ServiceId == serviceId)
                                    .ToListAsync();
            if (servicesMetadata.Count() == 0)
                return null;
            return servicesMetadata;
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

        public async Task<bool> IsResTypeExist(int resTypeId)
        {
            var resourceExist = await _context.Set<ResourceType>().FindAsync(resTypeId);
            if (resourceExist == null)
                return false;
            return true;
        }

        public async Task<bool> IsServiceExis(int serviceId)
        {
            var serviceExist = await _context.Set<Service>().FindAsync(serviceId);
            if (serviceExist == null)
                return false;
            return true;
        }
    }
       
}
