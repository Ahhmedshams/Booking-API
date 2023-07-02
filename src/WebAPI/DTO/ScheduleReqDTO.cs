namespace WebAPI.DTO
{
    public class ScheduleReqDTO
    {
        public int ResourceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
