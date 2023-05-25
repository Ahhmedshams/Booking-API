using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceDataMapper :Profile
    {
        public ResourceDataMapper()
        {
            CreateMap<ResourceData,ResourceDataRespIDValueDTO>().ReverseMap();
        }
    }
}
