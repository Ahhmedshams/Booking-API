using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceMapper: Profile
    {
        public ResourceMapper()
        {
            CreateMap<Resource, ResourceRespDTO>();
            CreateMap<ResourceReqDTO,Resource>().ReverseMap(); 
            CreateMap<Resource, ResourceWithDataDTO>().ReverseMap(); 
        }
    }
}
