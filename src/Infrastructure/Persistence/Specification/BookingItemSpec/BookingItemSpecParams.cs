namespace Infrastructure.Persistence.Specification.BookingItemSpec
{
    public class BookingItemSpecParams: PagingParams
    {
        public int? BookId { get; set; }
        public int? ResourceId { get; set; }
        public decimal? Price { get; set; }
    }
}
