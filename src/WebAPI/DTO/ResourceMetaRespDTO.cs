using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class ResourceMetaReqDTO
    {
        [Required]
        public int ResourceTypeId { get; set; }
        [Required]
        public string AttributeName { get; set; }
        [Required]
        public string AttributeType { get; set; }
    }

    public class ResourceAttribute
    {
        [Required]
        public string AttributeName { get; set; }
        [Required]
        public string AttributeType { get; set; }
    }


    public class ResourceMetaRespDTO
    {

        public int AttributeId { get; set; }

        [Required]
        public int ResourceTypeId { get; set; }
        [Required]
        public string AttributeName { get; set; }
        [Required]
        public string AttributeType { get; set; }
    }
}
