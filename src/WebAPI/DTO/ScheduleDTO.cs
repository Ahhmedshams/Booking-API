namespace WebAPI.DTO
{
    public class ScheduleDTO
    {
        public int ScheduleID { get; set; }
        public int ResourceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        //public IEnumerable<ScheduleItemDTO> ScheduleItems { get; set; }
    }
}
