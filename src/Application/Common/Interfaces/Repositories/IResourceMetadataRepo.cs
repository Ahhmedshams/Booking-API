using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IResourceMetadataRepo : IAsyncRepository<ResourceMetadata> , IRepository<ResourceMetadata>
    {
        Task<IEnumerable<ResourceMetadata>> AddRangeAsync(IEnumerable<ResourceMetadata> entities);
        IEnumerable<ResourceMetadata> AddRange(IEnumerable<ResourceMetadata> entities);
        IEnumerable<ResourceMetadata> Find(Expression<Func<ResourceMetadata, bool>> predicate);
        Task<IEnumerable<ResourceMetadata>> FindAsync(Expression<Func<ResourceMetadata, bool>> predicate);

    }
}
