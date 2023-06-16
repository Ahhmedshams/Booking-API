using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class AvailableResources
    {
        //public int ResourceId { get; set; }
        //public string Name { get; set; }
        //public DateTime Day { get; set; }
        //public TimeOnly StartTime { get; set; }
        //public TimeOnly EndTime { get; set; }

        public int id { get; set; }
        public int ResourceTypeId { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string Name { get; set; }
    }
}
