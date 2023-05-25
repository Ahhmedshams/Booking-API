using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IResourceRepo : IAsyncRepository<Resource>, IRepository<Resource>
    {
        Resource EditPrice(int id, decimal price);
        bool IsExist(int id);

    }
}
