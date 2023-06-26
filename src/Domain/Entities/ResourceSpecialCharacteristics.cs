using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourceSpecialCharacteristics : BaseEntity
    {
        public int ID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "TotalCapacity must be at least 1.")]
        public int TotalCapacity { get; set; }

        private int _availableCapacity;
        public int AvailableCapacity
        {
            get
            {
                return _availableCapacity;
            }
            set
            {
                _availableCapacity = Math.Min(Math.Max(value, 0), TotalCapacity);
            }
        }

        [ForeignKey("scheduleItem")]
        public int? ScheduleID { get; set; }

        [ForeignKey("Resource")]
        public int ResourceID { get; set; }
        public Resource Resource { get; set; }
        public ScheduleItem ScheduleItem { get; set; }
        public ResourceSpecialCharacteristics()
        {
            AvailableCapacity = TotalCapacity;
        }
    }

}
