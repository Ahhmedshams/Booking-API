using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class RegionGetDTO
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

    }
}
