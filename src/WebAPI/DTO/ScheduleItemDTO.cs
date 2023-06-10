namespace WebAPI.DTO
{
    public class ScheduleItemDTO
    {
        public int ScheduleId { get; set; }
        public DateTime Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool Available { get; set; }
        public bool Shift { get; set; }
    }
    public class SmallScheduleItemDTO
    {
        public DateTime Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
