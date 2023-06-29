using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    [Keyless]
    public class AvailableTimes
    {
        [NotMapped]
        public TimeOnly StartTime { get; set; }
        [NotMapped]

        public TimeOnly EndTime { get; set; }
    }
}
