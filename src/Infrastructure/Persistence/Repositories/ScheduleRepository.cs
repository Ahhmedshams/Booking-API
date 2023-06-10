using Application.Common.Models;

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
            if (schedule == null) return null;
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
                _context.SaveChangesAsync();
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
        public List<AvailableResources> GetAvailableResources(DateTime fromDate, DateTime toDate, int resourceTypeId)
        {
            var query = from r in _context.Resource
                        join rt in _context.ResourceTypes on r.ResourceTypeId equals rt.Id
                        join s in _context.Schedule on r.Id equals s.ResourceId into sGroup
                        from s in sGroup.DefaultIfEmpty()
                        join si in _context.ScheduleItem on s.ScheduleID equals si.ScheduleId into siGroup
                        from si in siGroup.DefaultIfEmpty()
                        where rt.Id == resourceTypeId
                              && (s.FromDate <= toDate && s.ToDate >= fromDate)
                              && si.Available == true
                        select new AvailableResources
                        {
                            ResourceId = r.Id,
                            Name = r.Name,
                            Day = si.Day,
                            StartTime = si.StartTime,
                            EndTime = si.EndTime
                        };

            /*            var distinctQuery = query.Distinct();
            */
            var result = query.ToList();
            return result;
        }

        public bool IsExist(int id)
        {
            return _context.Schedule.Any(res => res.ScheduleID == id);
        }
    }
}
