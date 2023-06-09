namespace WebAPI.DTO
{
    public class ClientBookingDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }
        public decimal TotalCost { get; set; }
        public string UserId { get; set; }
        public int ServiceId { get; set; }
    }


}
