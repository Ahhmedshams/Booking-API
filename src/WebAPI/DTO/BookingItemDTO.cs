namespace WebAPI.DTO
{
    public class BookingItemResDTO
    {
        public string ClientName { get; set; }
        public string ResourceName { get; set; }
        public decimal Price { get; set; }
    }

    public class BookingItemReqDTO
    {
        public int BookingId { get; set; }
        public int ResourceId { get; set; }
        public decimal Price { get; set; }
    }


}
