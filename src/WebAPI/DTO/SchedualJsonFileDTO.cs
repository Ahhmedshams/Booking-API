namespace WebAPI.DTO
{
    public class SchedualJsonFileDTO
    {
        ScheduleDTO Schedule { get; set; }
        IEnumerable<ScheduleItemDTO> ScheduleItems = new List<ScheduleItemDTO>();
    }

}
