using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IResourceTypeRepo : IRepository<ResourceType> , IAsyncRepository<ResourceType>
    {
        bool IsExist(int id);
        Task<bool> IsExistAsync(int id);

    }
}
