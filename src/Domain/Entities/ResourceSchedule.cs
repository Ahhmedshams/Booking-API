using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourceSchedule
    {
        public int Id { get; set; }
        public string Days { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set;}
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set;}

    }
}
