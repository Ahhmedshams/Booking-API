using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IScheduleItemRepo :IAsyncRepository<ScheduleItem>, IRepository<ScheduleItem>
    {
        bool IsExist(int id);
    }
}
