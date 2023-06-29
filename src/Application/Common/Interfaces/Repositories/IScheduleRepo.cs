using Application.Common.Models;
using Sieve.Models;
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

        List<AvailableSchedule> GetSchedules(DateTime fromDate, DateTime toDate, SieveModel sieveMode);
		Task<List<Resource>> GetAvailableResources(string _day, int _serviceId, string _startTime, string _endTime, SieveModel sieveMode, int? regionId = null);

        Task<List<int>> AddBulk(List<ScheduleJson> scheduleJsons);
        Task<List<AvailableTimes>> GetAvailableTimes(string _day, int _serviceId, int? regionId = null);
    }
}
