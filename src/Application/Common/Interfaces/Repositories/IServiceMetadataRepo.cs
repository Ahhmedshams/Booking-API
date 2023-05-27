using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IServiceMetadataRepo: IAsyncRepository<ServiceMetadata>
    {
        Task<IEnumerable<ServiceMetadata>> AddBulk(IEnumerable<ServiceMetadata> serviceMetadata);
        Task<ServiceMetadata> GetServiceMDByIdAsync(int serviceId, int resId, params Expression<Func<ServiceMetadata, object>>[] includes);
        Task<IEnumerable<ServiceMetadata>> GetByServiceId(int serviceId, params Expression<Func<ServiceMetadata, object>>[] includes);
        Task<IEnumerable<ServiceMetadata>> GetByResourceId(int resId, params Expression<Func<ServiceMetadata, object>>[] includes);

        Task<ServiceMetadata> EditServiceMDAsyn(int serviceId, int resId, ServiceMetadata entity);
        Task<ServiceMetadata> DeleteServiceMDAsyn(int serviceId, int resId);
        Task DeleteBulk(int serviceId);

        Task<bool> IsServiceExis(int serviceId);
        Task<bool> IsResTypeExist(int resTypeId);
        //Task<int> CheckExistenceOfServiceIdAndResId(int serviceId, int resTypeId);
        Task<bool> CheckDuplicateKey(int serviceId, int resTypeId);
    }
}
