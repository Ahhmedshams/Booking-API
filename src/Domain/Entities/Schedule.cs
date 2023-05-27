using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Schedule :BaseEntity
    {
        public int ScheduleID { get; set; }
        public string ResourceId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }

       public IEnumerable<ScheduleItem> ScheduleItem { get; set; }
    }
}
