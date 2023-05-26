using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IServiceMetadataRepo: IAsyncRepository<ServiceMetadata>
    {
        Task<ServiceMetadata> GetServiceMDByIdAsync(int serviceId, int resId, params Expression<Func<ServiceMetadata, object>>[] includes);

        Task<ServiceMetadata> EditServiceMDAsyn(int serviceId, int resId, ServiceMetadata entity);
        Task<ServiceMetadata> DeleteServiceMDAsyn(int serviceId, int resId);
    }
}
