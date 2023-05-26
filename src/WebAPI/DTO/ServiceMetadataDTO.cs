namespace WebAPI.DTO
{
    public class ServiceMetadataResDTO
    {
        public int ServiceId { get; set; }
        public int ResourceTypeId { get; set; }
        public string ServiceName { get; set; }
        public string ResourceTypeName { get; set; }
    }

    public class ServiceMetadataReqDTO
    {
        public int ServiceId { get; set; }
        public int ResourceTypeId { get; set; }
    }
}
