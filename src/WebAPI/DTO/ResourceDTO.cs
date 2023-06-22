using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class ResourceRespDTO
    {
        public int Id { get; set; }
        public int ResourceTypeId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public Decimal Price { get; set; }
    }

    public class ResourceReqDTO
    {
        public int ResourceTypeId { get; set; }
        public Decimal Price { get; set; }
        public string Name { get; set; }

    }

    public class ResourceWithDataDTO
    {
        public int ResourceTypeId { get; set; }
        public Decimal Price { get; set; }
        public string Name { get; set; }

        public List<ResourceDataRespIDValueDTO> ResourceAttributes { get; set; }

    }

   

}
