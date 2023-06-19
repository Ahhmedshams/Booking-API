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
        public int Id { get; set; }
        public string Uri { get; set; }
    }

    public class ServiceImage : ImageEntity{}

    public class ResourceTypeImage : ImageEntity { }

    public class ResourceImage : ImageEntity { }
}
