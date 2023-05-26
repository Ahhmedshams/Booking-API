namespace Domain.Entities
{
    public class BookingItem: BaseEntity
    {
        public decimal Price { get; set; }
        public int BookingId { get; set; }
        public int ResourceId { get; set; }
        public ClientBooking ClientBooking { get; set; }
        public Resource Resource { get; set; }
    }
}
