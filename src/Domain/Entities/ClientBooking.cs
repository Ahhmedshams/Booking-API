
using Domain.Identity;

namespace Domain.Entities
{
    public enum BookingStatus
    {
        Confirmed,
        Cancelled,
        Pending,
        Completed,
        InProcess
    }

    public class ClientBooking: BaseEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public string UserId { get; set; }

        public decimal TotalCost { get; set; }
        public ApplicationUser User { get; set; }

        public PaymentTransaction paymentTransaction { get; set; }
        public ICollection<BookingItem> BookingItems { get; set; } = new HashSet<BookingItem>();
    }
}
