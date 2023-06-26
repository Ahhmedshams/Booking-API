using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceSpecialCharacteristicsMapper: Profile
    {
        public ResourceSpecialCharacteristicsMapper()
        {
            CreateMap<ResourceSpecialCharacteristics, ResourceSpecialCharacteristicsDTO>().ForMember(RSC=>RSC.Day,opt=>opt.MapFrom(RSCEntity=>RSCEntity.ScheduleItem.Day)).ReverseMap();
        }
    }
}
