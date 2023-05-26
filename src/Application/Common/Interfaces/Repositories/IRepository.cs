using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById<IDType>(IDType id, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAll(bool withNoTracking = true, params Expression<Func<T, object>>[] includes);
        T Add(T entity);
        T Delete<IDType>(IDType id);
        T Edit<IDType>(IDType id, T entity, Expression<Func<T, IDType>> keySelector);
         T SoftDelete<IDType>(IDType id);
    }
}
