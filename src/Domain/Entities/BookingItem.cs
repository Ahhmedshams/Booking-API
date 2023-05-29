using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class BookingItem: BaseEntity
    {
        public decimal Price { get; set; }
        [ForeignKey("ClientBooking")]
        public int BookingId { get; set; }
        [ForeignKey("Resource")]
        public int ResourceId { get; set; }
        public ClientBooking ClientBooking { get; set; }
        public Resource Resource { get; set; }
    }
}
