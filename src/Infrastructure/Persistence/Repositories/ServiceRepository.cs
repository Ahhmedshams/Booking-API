using Application.Common.Helpers;
using Infrastructure.Persistence.Specification;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ServiceRepository : CRUDRepositoryAsync<Service>, IServiceRepo
    {
        public ServiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetAllServices()
        {
            var services = await _context.Set<Service>().Include(s=>s.Images)
                                    .Where(s => s.IsDeleted == false)
                                    .ToListAsync();
            return services;
        }

        public async Task<Service> GetServiceById(int id)
        {
            var service = await _context.Set<Service>().Include(s => s.Images)
                                    .Where(s => s.IsDeleted == false & s.Id == id)
                                    .FirstOrDefaultAsync();
            return service;
        }

        public async Task<IEnumerable<Service>> GetAllServicesWithSpec(ISpecification<Service> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<Service> GetServiceByIdWithSpec(ISpecification<Service> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }


        public async Task DeleteSoft(int id)
        {
            var service = await GetByIdAsync(id);
            service.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        private IQueryable<Service> ApplySpecification(ISpecification<Service> spec)
        {
            var query = _context.Set<Service>().Include(s => s.Images);
			return SpecificationEvaluator<Service>.GetQuery(query, spec);
        }
        public async Task<IEnumerable<Service>> ServicesByRegion(int RegionId)
        {
            var result =  _context.Set<Service>()
                .FromSqlRaw("EXEC FindMatchingServiceId @RegionId",
                    new SqlParameter("@RegionId", RegionId)).IgnoreQueryFilters().ToList();

            return result;
            
        }
    }
}
