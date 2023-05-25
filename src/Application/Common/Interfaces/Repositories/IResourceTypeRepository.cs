using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IResourceMetadataRepository : IAsyncRepository<ResourceMetadata>
    {
        Task<IEnumerable<ResourceMetadata>> AddRangeAsync(IEnumerable<ResourceMetadata> entities);
        Task DeleteBulkAsync(Expression<Func<ResourceMetadata, bool>> predicate);
        IEnumerable<ResourceMetadata> AddRange(IEnumerable<ResourceMetadata> entities);
        void DeleteBulk(Expression<Func<ResourceMetadata, bool>> predicate);

    }
}
