namespace Domain.Entities
{
    public class ResourceData: BaseEntity
    {
        public int ResourceId { get; set; } //foreignKey ==>partial Key
        public int AttributeId { get; set; } //foreignKey ==>partial Key

        public string AttributeValue { get; set; }

        public ResourceMetadata ResourceMetadata { get; set; }
        public Resource Resource { get; set; }

    }
}
