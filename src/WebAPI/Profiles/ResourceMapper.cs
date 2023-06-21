using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceMapper: Profile
    {
        public ResourceMapper()
        {
            CreateMap<Resource, ResourceRespDTO>()
                .ForMember(r => r.RegionName ,opt => opt.MapFrom( r => r.Region.Name ));
            CreateMap<Resource, ResourceReqDTO>().ReverseMap(); 
            CreateMap<Resource, ResourceWithDataDTO>().ReverseMap(); 
        }
    }
}
