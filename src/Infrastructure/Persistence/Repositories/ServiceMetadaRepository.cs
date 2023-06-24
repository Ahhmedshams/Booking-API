
using Application.Common.Helpers;
using Infrastructure.Persistence.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class ServiceMetadaRepository : CRUDRepositoryAsync<ServiceMetadata>, IServiceMetadataRepo
    {
        public ServiceMetadaRepository(ApplicationDbContext context) : base(context)
        {
        }


		public async Task AddOne(ServiceMetadata serviceMetadata)
		{
			await _context.Set<ServiceMetadata>().AddAsync(serviceMetadata);
		}
		public async Task AddBulk(IEnumerable<ServiceMetadata> serviceMetadata)
        {
            await _context.Set<ServiceMetadata>().AddRangeAsync(serviceMetadata);
            await _context.SaveChangesAsync();
        }

        public async Task EditOne(int serviceId, int resId)
        {
            await DeleteOne(serviceId, resId);
            ServiceMetadata serviceMD = new();
            serviceMD.ServiceId = serviceId;
            serviceMD.ResourceTypeId = resId;
            await AddAsync(serviceMD);


        }

        public async Task EditBulk(int serviceId, IEnumerable<int> resIds)
        {
            await DeleteBulk(serviceId);
            List<ServiceMetadata> serviceMetadatas = new();
            ServiceMetadata serviceMD = new();
            foreach (int resId in resIds)
            {
                serviceMD.ServiceId = serviceId;
                serviceMD.ResourceTypeId = resId;
                serviceMetadatas.Add(serviceMD);
            }
            await AddBulk(serviceMetadatas);

        }

        public async Task<ServiceMetadata> DeleteOne(int serviceId, int resId)
        {
            var foundEntity = await _context.Set<ServiceMetadata>()
                    .FirstOrDefaultAsync(s=>s.ServiceId == serviceId &&
                                        s.ResourceTypeId == resId && 
                                        s.IsDeleted == false);

            if (foundEntity == null)
                return null;

            _context.Set<ServiceMetadata>().Remove(foundEntity);
            await _context.SaveChangesAsync();

            return foundEntity;
        }
        
        public async Task DeleteBulk(int serviceId)
        {
            await _context.Set<ServiceMetadata>().Where(s => s.ServiceId == serviceId).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }


        public async Task<bool> CheckDuplicateKey(int serviceId, int resTypeId)
        {
            var objectExist = await GetById(serviceId, resTypeId);
            if (objectExist == null)
                return false;
            return true;
        }

        public async Task<IEnumerable<ServiceMetadata>> GetByResourceId(int resId, params Expression<Func<ServiceMetadata, object>>[] includes)
        {
            var servicesMetadata = await _context.Set<ServiceMetadata>()
                                    .Where(s => s.ResourceTypeId == resId&&
                                           s.IsDeleted == false)
                                    .ToListAsync();
            if (servicesMetadata.Count() == 0)
                return null;
            return servicesMetadata;
        }

        public async Task<IEnumerable<ServiceMetadata>> GetByServiceId(int serviceId, params Expression<Func<ServiceMetadata, object>>[] includes)
        {
            var servicesMetadata  = await _context.Set<ServiceMetadata>()
                                    .Where(s => s.ServiceId == serviceId &&
										   s.IsDeleted == false)
                                    .ToListAsync();
            if (servicesMetadata.Count() == 0)
                return null;
            return servicesMetadata;
        }

        public async Task<ServiceMetadata> GetById(int serviceId, int resId, params Expression<Func<ServiceMetadata, object>>[] includes)
        {
            var query = _context.Set<ServiceMetadata>().Where(s => s.IsDeleted==false).AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);

            }
            return await _context.Set<ServiceMetadata>().FirstOrDefaultAsync(s=> s.ServiceId == serviceId &&
																			 s.ResourceTypeId == resId &&
																			 s.IsDeleted == false);
        }

        public async Task<bool> IsResTypeExist(int resTypeId)
        {
            var resourceExist = await _context.Set<ResourceType>().FirstOrDefaultAsync(s => s.Id == resTypeId && s.IsDeleted == false);
            if (resourceExist == null)
                return false;
            return true;
        }

        public async Task<bool> IsServiceExis(int serviceId)
        {
            var serviceExist = await _context.Set<Service>().FirstOrDefaultAsync(s => s.Id == serviceId && s.IsDeleted == false);
            if (serviceExist == null)
                return false;
            return true;
        }

        public async Task<IEnumerable<ServiceMetadata>> GetAllServiceMDWithSpec(ISpecification<ServiceMetadata> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        private IQueryable<ServiceMetadata> ApplySpecification(ISpecification<ServiceMetadata> spec)
        {
            return SpecificationEvaluator<ServiceMetadata>.GetQuery(_context.Set<ServiceMetadata>(), spec);
        }

		
	}
       
}
