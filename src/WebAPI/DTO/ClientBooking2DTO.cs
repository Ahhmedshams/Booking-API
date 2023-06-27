using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class ClientBooking2DTO
    {
        public int ?Id { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Location { get; set; }
        public BookingStatus ?Status { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public decimal? TotalCost { get; set; }
        public string UserID { get; set; }
        public int ServiceID { get; set; }
        public List<int> ResourceIDs { get; set; }
    }
}
