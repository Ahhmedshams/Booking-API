﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ScheduleItem : BaseEntity
    {
        public int ScheduleID { get; set; }  
        public string Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set;}
        public bool Available { get; set; }
        public Schedule ScheduleData { get; set; }

    }
}