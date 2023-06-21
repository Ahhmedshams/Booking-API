using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Resource: BaseEntity
    {
        public int Id { get; set; } 
        public int ResourceTypeId { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }

        public ResourceType ResourceType { get; set; }
        public IEnumerable<Schedule> Schedules { get; set; }
        public ResourceSpecialCharacteristics? ResourceSpecialCharacteristics { get; set; }
    }
}
