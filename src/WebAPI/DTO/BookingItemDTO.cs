using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class BookingItemDTO
    {
        public int BookingId { get; set; }
        public int ResourceId { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public decimal Price { get; set; }
    }


}
