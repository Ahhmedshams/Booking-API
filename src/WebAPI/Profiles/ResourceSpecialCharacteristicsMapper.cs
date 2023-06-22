using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceSpecialCharacteristicsMapper: Profile
    {
        public ResourceSpecialCharacteristicsMapper()
        {
            CreateMap<ResourceSpecialCharacteristics, ResourceSpecialCharacteristicsDTO>().ReverseMap();
        }
    }
}
