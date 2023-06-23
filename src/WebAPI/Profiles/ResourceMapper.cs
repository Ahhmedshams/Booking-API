using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceMapper: Profile
    {
        public ResourceMapper()
        {
            CreateMap<Resource, ResourceRespDTO>();
              //.ForMember(dest => dest.RegionName, opt =>
              //{
              //    opt.PreCondition(src => src.Region != null);
              //    opt.MapFrom(src => src.Region.Name);
              //});
            CreateMap<Resource, ResourceReqDTO>().ReverseMap(); 
            CreateMap<Resource, ResourceWithDataDTO>().ReverseMap(); 
        }
    }
}
