using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IResourceMetadataRepo : IAsyncRepository<ResourceMetadata> , IRepository<ResourceMetadata>
    {
        Task<IEnumerable<ResourceMetadata>> AddRangeAsync(IEnumerable<ResourceMetadata> entities);
        Task DeleteBulkAsync(Expression<Func<ResourceMetadata, bool>> predicate);
        IEnumerable<ResourceMetadata> AddRange(IEnumerable<ResourceMetadata> entities);
        void DeleteBulk(Expression<Func<ResourceMetadata, bool>> predicate);
        IEnumerable<ResourceMetadata> Find(Expression<Func<ResourceMetadata, bool>> predicate);
        Task<IEnumerable<ResourceMetadata>> FindAsync(Expression<Func<ResourceMetadata, bool>> predicate);

    }
}
