using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourceSpecialCharacteristics:BaseEntity
    {
        public int ID { get; set; }
        public int TotalCapacity { get; set; }
        public int AvailableCapacity { get; set; }
        [ForeignKey("scheduleItem")]
        public int? ScheduleID { get; set; }

        [ForeignKey("Resource")]
        public int ResourceID { get; set; }
        public Resource Resource { get; set; }
        public ScheduleItem scheduleItem { get; set; }
    }

}
