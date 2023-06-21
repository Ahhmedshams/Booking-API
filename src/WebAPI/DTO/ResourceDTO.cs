namespace WebAPI.DTO
{
    public class ResourceRespDTO
    {
        public int Id { get; set; }
        public int ResourceTypeId { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }

        public string RegionName { get; set; }
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
