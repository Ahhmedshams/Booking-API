using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ServiceMetadata: BaseEntity
    {
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("ResourceType")]
        public int ResourceTypeId { get; set; }
        public int NoOfResources { get; set; }
        public ResourceType ResourceType { get; set; }
    }
}
