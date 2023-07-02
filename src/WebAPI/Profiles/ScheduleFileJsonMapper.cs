using Application.Common.Models;
using AutoMapper;

namespace WebAPI.Profiles
{
    public class ScheduleFileJsonMapper: Profile
    {
        public ScheduleFileJsonMapper() 
        { 
            CreateMap<SchedualJsonFileDTO, ScheduleJson>().ReverseMap();
            
        }
    }
}
