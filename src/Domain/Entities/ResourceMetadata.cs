namespace Domain.Entities
{
    public class ResourceMetadata: BaseEntity
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeType { get; set; }
        public int ResourceTypeId { get; set; }
        public ResourceType ResourceType { get; set; }

    }
}
