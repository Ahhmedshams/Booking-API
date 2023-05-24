namespace Domain.Entities
{
    public class ResourceMetadata
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeType { get; set; }
        public string ResourceTypeId { get; set; }
        public ResourceType ResourceType { get; set; }

    }
}
