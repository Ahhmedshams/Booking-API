using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class ServiceRegionDTO
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
