using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourcePrice: BaseEntity
    {
        public int Id { get; set; } 
        public string ResourceId { get; set; }
        public int ResourceTypeId { get; set; }
        public Decimal Price { get; set; }



        public ResourceData ResourceData { get; set; }
        public ResourceType ResourceType { get; set; }
    }
}
