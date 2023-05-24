using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById<IDType>(IDType id);
        IEnumerable<T> GetAll(bool withNoTracking = true);
        T Add(T entity);
        T Delete<IDType>(IDType id);
        T Edit<IDType>(IDType id, T entity);
    }
}
