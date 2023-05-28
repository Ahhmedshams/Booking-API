namespace WebAPI.DTO
{
    public class ClientBookingDTO
    {
        public int Id { get; set;}
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
    }

}
