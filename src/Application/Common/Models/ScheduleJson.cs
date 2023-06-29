using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class ScheduleJson
    {
        public Schedule schedule { get; set; }

        public List<ScheduleItem> scheduleItems { get; set; }
    }
}
