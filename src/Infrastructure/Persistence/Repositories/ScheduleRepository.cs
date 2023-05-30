using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ScheduleRepository : CRUDRepository<Schedule>, IScheduleRepo
    {
        public ScheduleRepository(ApplicationDbContext context) : base(context)
        {
        }


        public bool IsExist(int id)
        {
            return _context.Schedule.Any(res => res.ScheduleID == id);
                
               
        }
    }
}
