using AutoMapper;
using WebAPI.DTO;

namespace WebAPI.Profiles
{
    public class ServiceMetadataMapper : Profile
    {
        public ServiceMetadataMapper()
        {
            
            CreateMap<ServiceMetadata, ServiceMetadataDTO>().ReverseMap();
            CreateMap<ServiceMetadata, ServiceMDReqDTO>().ReverseMap();
        }
    }
}
