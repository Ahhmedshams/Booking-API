namespace WebAPI.DTO
{
    public class ServiceMetadataDTO
    {
        public int ServiceId { get; set; }
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
    }

    public class ServiceMDReqDTO
    {
        public int ResourceTypeId { get; set; }
    }

}
