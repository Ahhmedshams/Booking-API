using Application.Common.Helpers;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IServiceMetadataRepo: IAsyncRepository<ServiceMetadata>
    {
        Task<IEnumerable<ServiceMetadata>> GetAllServiceMDWithSpec(ISpecification<ServiceMetadata> spec);
        Task AddBulk(IEnumerable<ServiceMetadata> serviceMetadata);
        Task AddOne(ServiceMetadata serviceMetadata);
        Task EditOne(int serviceId, int resId);
        Task EditBulk(int serviceId,IEnumerable<int> resIds);

        Task<ServiceMetadata> GetById(int serviceId, int resId, params Expression<Func<ServiceMetadata, object>>[] includes);
        Task<IEnumerable<ServiceMetadata>> GetByServiceId(int serviceId, params Expression<Func<ServiceMetadata, object>>[] includes);
        Task<IEnumerable<ServiceMetadata>> GetByResourceId(int resId, params Expression<Func<ServiceMetadata, object>>[] includes);

        
        Task<ServiceMetadata> DeleteOne(int serviceId, int resId);
        Task DeleteBulk(int serviceId);

        Task<bool> IsServiceExis(int serviceId);
        Task<bool> IsResTypeExist(int resTypeId);
        Task<bool> CheckDuplicateKey(int serviceId, int resTypeId);
    }
}
