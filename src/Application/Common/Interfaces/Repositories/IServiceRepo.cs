using Application.Common.Helpers;

namespace Application.Common.Interfaces.Repositories
{
    public interface IServiceRepo: IAsyncRepository<Service>
    {
        Task<IEnumerable<Service>> GetAllServices();
        Task<Service> GetServiceById(int id);
        Task<IEnumerable<Service>> GetAllServicesWithSpec(ISpecification<Service> spec);
        Task<Service> GetServiceByIdWithSpec(ISpecification<Service> spec);
        Task DeleteSoft(int id);
        Task<IEnumerable<Service>> ServicesByRegion(int RegionId);
    }
}
