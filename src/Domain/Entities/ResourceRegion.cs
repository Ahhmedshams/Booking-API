namespace Domain.Entities
{
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    public class ResourceRegion
    {
        public int ResourceId { get; set; }
        public int RegionId { get; set; }
        public Resource Resource { get; set; }
        public Region Region { get; set; }
        

    }
}
