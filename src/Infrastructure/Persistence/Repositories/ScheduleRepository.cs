using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Persistence.Repositories
{
    public class ScheduleRepository : CRUDRepository<Schedule>, IScheduleRepo
    {
		private readonly IResourceRepo _resourceRepo;

		public ScheduleRepository(ApplicationDbContext context,IResourceRepo resourceRepo) : base(context)
        {
			_resourceRepo = resourceRepo;
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

        public List<AvailableSchedule> GetSchedules(DateTime fromDate, DateTime toDate,SieveModel sieveModel)
        {
            var schedules = _context.Schedule
                .Where(s => s.FromDate >= fromDate && s.ToDate <= toDate)
                .GroupBy(s => new { s.FromDate, s.ToDate })
                .ToList()
                .Select(g => new AvailableSchedule { FromDate = g.Key.FromDate, ToDate = g.Key.ToDate });

            return schedules.ToList();
        }

       /* public List<AvailableSchedule> GetSchedules(DateTime fromDate, DateTime toDate)
        {

            var schedules = _context.Schedule
                .Where(s => s.FromDate >= fromDate && s.ToDate <= toDate)
                .GroupBy(s => new { s.FromDate, s.ToDate })
                .Select(g => g.First())
                .Select(s => new AvailableSchedule { FromDate = s.FromDate, ToDate = s.ToDate });
      
            return schedules.ToList();
        }*/

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
        public async Task<List<Resource>> GetAvailableResources(string _day,int _serviceId ,string _startTime, string _endTime, SieveModel sieveModel, int? regionId = null)
        {
            try
            {
                var day = _day;
                var startTime = _startTime;
                var endTime = _endTime;
                int serviceId = _serviceId;

                var results = _context.Resource
                    .FromSqlRaw("EXEC GetAvailableResourceForService @param1, @param2 ,@param3,@param4,@RegionId",
                        new SqlParameter("@param1", day),
                        new SqlParameter("@param2", serviceId),
                        new SqlParameter("@param3", startTime),
                        new SqlParameter("@param4", endTime),
                        new SqlParameter("@RegionId", regionId)
                        )
					  .IgnoreQueryFilters()
                      .ToList();

                if (results.Count > 0)
                {
                    List<Resource> resources = new List<Resource>();
                    foreach(var res in results)
                    {
                        Resource resource = _resourceRepo.GetResById(res.Id);
                        resources.Add(resource);
					}

                    return resources;
                }
                else
                {
                    return new List<Resource>();
                }
            }
            catch (Exception ex) 
            {
                return new List<Resource>();
            }
                

           
        }

        public bool IsExist(int id)
        {
            return _context.Schedule.Any(res => res.ScheduleID == id);
        }
    }
}
