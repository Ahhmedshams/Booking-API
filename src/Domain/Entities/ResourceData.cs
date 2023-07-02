using Sieve.Attributes;

namespace Domain.Entities
{
    public class ResourceData: BaseEntity
    {
        public int ResourceId { get; set; } //foreignKey ==>partial Key
        public int AttributeId { get; set; } //foreignKey ==>partial Key
        [Sieve(CanFilter = true, CanSort = true)]
        public string AttributeValue { get; set; }

        public ResourceMetadata ResourceMetadata { get; set; }
        public Resource Resource { get; set; }

    }
}
