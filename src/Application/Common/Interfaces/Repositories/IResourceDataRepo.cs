using Application.Common.Model;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IResourceDataRepo :IAsyncRepository<ResourceData> ,IRepository<ResourceData>
    {
        IEnumerable<ResourceData> Find(Expression<Func<ResourceData, bool>> predicate);
       Task< IEnumerable<ResourceData>> FindAsync(Expression<Func<ResourceData, bool>> predicate);
        Task<ResourceData> FindAsync(int ResourceId, int AttributeId);
        IEnumerable<ResourceData> AddRange(IEnumerable<ResourceData> entities);
        Task<IEnumerable<ResourceData>> AddRangeAsync(IEnumerable<ResourceData> entities);

        Task<AllResourceData> GetAllReourceData(int id);

        Task<List<AllResourceData>> GetAllData();
        Task<List<AllResourceData>> GetAllDataByType(int id);


        Task<bool> IsExistAsync(Expression<Func<ResourceData, bool>> predicate);

    }
}
