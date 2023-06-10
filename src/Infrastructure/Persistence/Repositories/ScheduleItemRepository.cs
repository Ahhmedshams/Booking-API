using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ScheduleItemRepository : CRUDRepository<ScheduleItem>, IScheduleItemRepo
    {
        public ScheduleItemRepository(ApplicationDbContext context) : base(context)
        {
        }
        public IEnumerable<ScheduleItem> AddRange(IEnumerable<ScheduleItem> entities)
        {
            _context.ScheduleItem.AddRange(entities);
            _context.SaveChanges();
            return entities;
        }

        public async Task<IEnumerable<ScheduleItem>> AddRangeAsync(IEnumerable<ScheduleItem> entities)
        {
            await _context.ScheduleItem.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public ScheduleItem Delete(int id, DateTime Day, TimeOnly startTime, TimeOnly endTime)
        {
            bool found = IsExist(id, Day, startTime, endTime);
            if (found)
            {
                ScheduleItem result =
           _context.ScheduleItem.Where(res => res.ScheduleId == id && res.Day == Day && res.StartTime == startTime && res.EndTime == endTime).FirstOrDefault();
                _context.ScheduleItem.Remove(result);
                _context.SaveChanges();
                return result;
            }
            return null;
        }

        public bool IsExist(int id, DateTime Day, TimeOnly startTime, TimeOnly endTime)
        {
            var result =
                _context.ScheduleItem.Where(res => res.ScheduleId == id && res.Day == Day && res.StartTime == startTime && res.EndTime == endTime);
            if (result != null)
                return true;
            //return (ScheduleItem)result;
            // return null;
            return false;

        }

        public async Task<ScheduleItem> FindAsync(DateTime day, TimeOnly startTime, TimeOnly endTime)
        {
            return await _context.ScheduleItem.FirstOrDefaultAsync(s => s.Day == day && s.StartTime == startTime && s.EndTime == endTime);
        }


        public bool IsExistWithId(int id)
        {
            return _context.Schedule.Any(res => res.ScheduleID == id);
        }
    }
}
