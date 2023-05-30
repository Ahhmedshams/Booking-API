using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IScheduleRepo : IAsyncRepository<Schedule>, IRepository<Schedule>
    {
        bool IsExist(int id);
    }
}
