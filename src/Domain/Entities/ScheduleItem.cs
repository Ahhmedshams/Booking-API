using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ScheduleItem : BaseEntity
    {
        public int ID { get; set; }
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public DateTime Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set;}
        public bool Shift { get; set;}
        public bool Available { get; set; }
        public ResourceSpecialCharacteristics? ResourceSpecialCharacteristics { get; set; }

    }
}
