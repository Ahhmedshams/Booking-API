using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourceData
    {
        public int ResourceId { get; set; } //Key
        public int AttributeId { get; set; } //foreignKey
        public int ResourceTypeId { get; set; } //foreignKey

        public string AttributeValue { get; set; }

        public ResourceMetadata ResourceMetadata { get; set; }
        public ResourceType ResourceType { get; set; }

    }
}
