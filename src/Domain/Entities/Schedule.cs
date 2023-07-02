using Sieve.Attributes;
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
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public IEnumerable<ScheduleItem> ScheduleItems { get; set; }
    }
}
