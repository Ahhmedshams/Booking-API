using AutoMapper;

namespace WebAPI.Profiles
{
    public class ScheduleItemMapper:Profile
    {
        public ScheduleItemMapper() 
        { 
            CreateMap<ScheduleItemDTO,ScheduleItem>().ReverseMap();
            CreateMap<ScheduleItemGetDTO,ScheduleItem>().ReverseMap();
        }
    }
}
