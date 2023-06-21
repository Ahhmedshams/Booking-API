namespace Domain.Entities
{
    public class ResourceType: BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Shown { get; set; }
        public bool HasSchedual { get; set; }
        public IEnumerable<ResourceMetadata> Metadata { get; set; }
        public ICollection<ResourceTypeImage> Images { get; set; }
    }
}
