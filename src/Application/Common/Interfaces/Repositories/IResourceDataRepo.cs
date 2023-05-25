using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IResourceDataRepo :IAsyncRepository<ResourceData> ,IRepository<ResourceData>
    {
        IEnumerable<ResourceData> Find(Expression<Func<ResourceData, bool>> predicate);
        IEnumerable<ResourceData> AddRange(IEnumerable<ResourceData> entities);
        Task<IEnumerable<ResourceData>> AddRangeAsync(IEnumerable<ResourceData> entities);

    }
}
