using AutoMapper;
using WebAPI.DTO;

namespace WebAPI.Profiles
{
    public class ServiceMetadataMapper : Profile
    {
        public ServiceMetadataMapper()
        {
            
            CreateMap<ServiceMetadata, ServiceMetadataDTO>()
                //.ForMember(des => des.ResourceTypeName, opt => opt.MapFrom(src => src.ResourceType.Name))
                .ReverseMap();

            CreateMap<ServiceMetadata, ServiceMDReqDTO>().ReverseMap();
        }
    }
}
