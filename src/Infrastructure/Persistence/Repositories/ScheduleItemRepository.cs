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

        public bool IsExist(int id)
        {
            throw new NotImplementedException();
        }
    }
}
