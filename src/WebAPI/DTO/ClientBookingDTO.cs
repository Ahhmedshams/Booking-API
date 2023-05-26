namespace WebAPI.DTO
{
    public class ClientBookingDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }
        public string User { get; set; }
        public string Service { get; set; }
    }

    public class ClientBookingReqDTO
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }
        public int User { get; set; }
        public int Service { get; set; }
    }


}
