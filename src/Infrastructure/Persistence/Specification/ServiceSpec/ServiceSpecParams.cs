namespace Infrastructure.Persistence.Specification.ServiceSpec
{
    public class ServiceSpecParams: PagingParams
    {
        public string? Sort { get; set; }
        public int? Id { get; set; }
        public string? Name {  get; set; }
    }
}
