using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IScheduleItemRepo :IAsyncRepository<ScheduleItem>, IRepository<ScheduleItem>
    {
        Task<IEnumerable<ScheduleItem>> AddRangeAsync(IEnumerable<ScheduleItem> entities);
        IEnumerable<ScheduleItem> AddRange(IEnumerable<ScheduleItem> entities);
        Task<ScheduleItem> FindAsync(int scheduleId,DateTime day, TimeOnly startTime, TimeOnly endTime);
        bool IsExist(int id, DateTime Day, TimeOnly startTime, TimeOnly endTime);
        bool IsExistWithId(int? id);
        ScheduleItem Delete(int id, DateTime day, TimeOnly startTime, TimeOnly endTime);
    }
}
