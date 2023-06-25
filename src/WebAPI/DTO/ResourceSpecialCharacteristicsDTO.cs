using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class ResourceSpecialCharacteristicsDTO
    {
        public int TotalCapacity { get; set; }

        public int AvailableCapacity { get; set; }

        public int ?ScheduleID { get; set; }

        public DateTime? Day { get; set; } 
        public string ResourceName { get; set; }
        public int ResourceID { get; set; }
    }
}
