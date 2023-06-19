using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public abstract class ImageEntity : BaseEntity
    {
        public string Uri { get; set; }
        public string Discriminator { get; set; }
    }

    [Table("Images")]
    public class ServiceImage : ImageEntity
    {
        public Service Service { get; set; }
    }

    [Table("Images")]
    public class ResourceTypeImage : ImageEntity
    {
        public ResourceType ResourceType { get; set; }
    }

    [Table("Images")]
    public class ResourceImage : ImageEntity
    {
        public Resource Resource { get; set; }
    }
}
