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
    public class EditScheduleItemDTO
    {
        public int ScheduleId { get; set; }

        public DateTime oldDay { get; set; }
        public TimeOnly oldStartTime { get; set; }
        public TimeOnly oldEndTime { get; set; }
        public DateTime newDay { get; set; }
        public TimeOnly newStartTime { get; set; }
        public TimeOnly newEndTime { get; set; }
        public bool Available { get; set; }
        public bool Shift { get; set; }

    }
    public class SmallScheduleItemDTO
    {
        public DateTime Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }

    public class ScheduleItemGetDTO
    {
        public int ScheduleId { get; set; }
        public DateTime Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool Available { get; set; }
        public bool Shift { get; set; }
        public string Name { get; set; }
        public ICollection<ResourceImage> ImageUrls { get; set; }
    }
}
