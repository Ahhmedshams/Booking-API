using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourceSchedule : BaseEntity
    {
        public int Id { get; set; }
        public string ResourceId { get; set; }
        public string Days { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set;}
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set;}

        public ResourceData ResourceData { get; set; }

    }
}
