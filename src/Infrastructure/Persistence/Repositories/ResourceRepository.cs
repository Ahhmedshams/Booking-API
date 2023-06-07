using Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceRepository : CRUDRepository<Resource>, IResourceRepo
    {
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
        }


        public bool IsExist(int id)
        {
            return _context.Resource.Any(res => res.Id == id);
        }
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var Resource = _context.Resource.Find(id);
            if (Resource == null)
                return false;


            Resource.IsDeleted = true;
           

            _context.ResourceData.Where(res => res.ResourceId == Resource.Id).ToList().ForEach(res =>
            res.IsDeleted = true
            );

            DeleteResourceSchedule(id);
            

            await _context.SaveChangesAsync();
            return true;
        }
        public Resource EditPrice(int id, decimal price)
        {
            var foundEntity = _context.Resource.Find(id);
            if (foundEntity == null) return null;
            foundEntity.Price = price;
               _context.SaveChanges();
            return foundEntity;
        }


        private void DeleteResourceSchedule(int id)
        {
            var Schedules = _context.Schedule.Where(sch => sch.ResourceId == id).ToList();


            foreach (var Schedule in Schedules)
            {
                Schedule.IsDeleted = true;
                _context.ScheduleItem.Where(sch => sch.ScheduleId == Schedule.ScheduleID).ToList().ForEach(schItem =>
                        schItem.IsDeleted = false
                );
            }
        }
    }
}
