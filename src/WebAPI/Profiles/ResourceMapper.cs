using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceMapper: Profile
    {
        public ResourceMapper()
        {
            CreateMap<Resource, ResourceRespDTO>().ReverseMap();
            CreateMap<Resource, ResourceReqDTO>().ReverseMap();
        }
    }
}
