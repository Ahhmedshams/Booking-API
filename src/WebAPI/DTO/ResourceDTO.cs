namespace WebAPI.DTO
{
    public class ResourceRespDTO
    {
        public int Id { get; set; }
        public int ResourceTypeId { get; set; }
        public Decimal Price { get; set; }
    }

    public class ResourceReqDTO
    {
        public int ResourceTypeId { get; set; }
        public Decimal Price { get; set; }
    }


}
