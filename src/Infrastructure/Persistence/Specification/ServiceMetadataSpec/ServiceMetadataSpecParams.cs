namespace Infrastructure.Persistence.Specification.ServiceMetadataSpec
{
    public class ServiceMetadataSpecParams: PagingParams
    {
        public int? ServiceId { get; set; }
        public int? ResourceTypeId { get; set; }
    }
}
