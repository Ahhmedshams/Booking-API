using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T?> GetByIdAsync<IDType>(IDType id , params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(bool withNoTracking = true, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task<T> DeleteAsync<IDType>(IDType id);
        Task<T> EditAsync<IDType>(IDType id, T entity, Expression<Func<T, IDType>> keySelector);
    }
}
