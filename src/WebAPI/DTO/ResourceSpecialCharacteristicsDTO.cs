using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class ResourceSpecialCharacteristicsDTO
    {

        public int ID { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        public int TotalCapacity { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        public int AvailableCapacity { get; set; }

        public int ?ScheduleID { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        public DateTime? Day { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        public string ResourceName { get; set; }
        public int ResourceID { get; set; }

    }


    public class ResourceSpecialCharacteristicsEditDTO
    {

        public int ID { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        public int TotalCapacity { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        public int AvailableCapacity { get; set; }

        public int? ScheduleID { get; set; }

        public int ResourceID { get; set; }

    }
}
