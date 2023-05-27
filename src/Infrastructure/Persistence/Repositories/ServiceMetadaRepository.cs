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

        public async Task<bool> CheckDuplicateKey(int serviceId, int resTypeId)
        {
            var objectExist = await GetServiceMDByIdAsync(serviceId, resTypeId);
            if (objectExist == null)
                return false;
            return true;
        }

        public async Task<int> CheckExistenceOfServiceIdAndResId(int serviceId, int resTypeId)
        {
            var serviceExist = await _context.Set<Service>().FindAsync(serviceId);
            if (serviceExist == null)
                return 1; //Service not exist

            var resourceExist = await _context.Set<ResourceType>().FindAsync(resTypeId);
            if (resourceExist == null)
                return -1; //Resource not exist

            return 0; //both are found
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
