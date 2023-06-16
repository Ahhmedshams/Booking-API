using Application.Common.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Persistence.Repositories
{
    public class ScheduleRepository : CRUDRepository<Schedule>, IScheduleRepo
    {
        public ScheduleRepository(ApplicationDbContext context) : base(context)
        {
           
        }

        public Schedule GetByResourceId(int resourceId)
        {
            Schedule schedule = _context.Schedule.Where(s => s.ResourceId == resourceId).FirstOrDefault();
           // if (schedule == null) return null;
            return schedule;
        }

        public async Task<string> ScheduleSoftDelete(int scheduleId)
        {
            var result = await SoftDeleteAsync(scheduleId);
            if (result == null)
                return null;
            var ScheduleItems = _context.ScheduleItem.Where(s => s.ScheduleId == scheduleId);
            if (ScheduleItems.Count() != 0)
            {
                SoftDelete(ScheduleItems);
                await _context.SaveChangesAsync(); //catch excpetion
                return ("Schedule And Schedule Items Deleted");
            }
            return ("Schedule Deleted");
        }

        public Schedule AddSchedule(Schedule schedule)
        {
            var rescourceFound = _context.Resource.Where(r => r.Id == schedule.ResourceId);
            if (rescourceFound != null)
            {
                var result = Add(schedule);
                return result;
            }
            return null;
        }
        public List<Resource> GetAvailableResources(string _day,int _serviceId ,string _startTime, string _endTime)
        {
            //string formatted = _day.ToString("yyyy-MM-dd");
            //string formatstart = _startTime.ToString("hh:mm:ss");
                var day = _day;
                var startTime = _startTime;
                var endTime = _endTime;
                int serviceId = 1;

                var results = _context.Resource
                    .FromSqlRaw("EXEC GetAvailableResourceForService @param1, @param2 ,@param3,@param4",
                        new SqlParameter("@param1", day),
                        new SqlParameter("@param2", serviceId),
                        new SqlParameter("@param3", startTime),
                        new SqlParameter("@param4", endTime)
                        )
                      .IgnoreQueryFilters()
                      .ToList();
                return results;

        }

        public bool IsExist(int id)
        {
            return _context.Schedule.Any(res => res.ScheduleID == id);
        }
    }
}
