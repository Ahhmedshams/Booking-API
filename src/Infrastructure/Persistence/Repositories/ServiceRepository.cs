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
            var services = await _context.Set<Service>()
                                    .Where(s => s.IsDeleted == false)
                                    .ToListAsync();
            return services;
        }

        public async Task<Service> GetServiceById(int id)
        {
            var service = await _context.Set<Service>()
                                    .Where(s => s.IsDeleted == false & s.Id == id)
                                    .FirstOrDefaultAsync();
            return service;
        }

        public async Task DeleteSoft(int id)
        {
            var service = await GetByIdAsync(id);
            service.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}
