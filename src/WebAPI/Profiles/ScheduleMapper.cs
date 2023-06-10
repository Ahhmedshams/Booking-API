using AutoMapper;

namespace WebAPI.Profiles
{
    public class ScheduleMapper:Profile
    {
        public ScheduleMapper()
        {
            CreateMap<Schedule, ScheduleDTO>().ReverseMap();
            CreateMap<Schedule, ScheduleReqDTO>().ReverseMap();
        }
    }
}
