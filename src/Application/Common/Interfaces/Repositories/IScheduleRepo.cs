using Application.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IScheduleRepo : IAsyncRepository<Schedule>, IRepository<Schedule>
    {
        Task<string> ScheduleSoftDelete(int scheduleId);
        Schedule GetByResourceId(int resourceId);
        bool IsExist(int id);
        Schedule AddSchedule(Schedule schedule);

        List<Resource> GetAvailableResources(string _day, int _serviceId, string _startTime, string _endTime);
    }
}
