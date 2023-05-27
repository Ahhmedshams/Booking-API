using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class BaseEntity
    {
        public bool IsDeleted { get; set; } 
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }


        //public string? LastUpdatedBy { get; set; }
        //public string? CreatedBy { get; set; }

        // TODO if we need this 
        //public string? CreatedById { get; set; }
        //public string? LastUpdatedById { get; set; }
    }
}
