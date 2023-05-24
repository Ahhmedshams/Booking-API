using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourceData: BaseEntity
    {
        public string ResourceId { get; set; } //partial Key
        public int AttributeId { get; set; } //foreignKey ==>partial Key

        public string AttributeValue { get; set; }

        public ResourceMetadata ResourceMetadata { get; set; }

    }
}
