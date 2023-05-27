using AutoMapper;

namespace WebAPI.Profiles
{
    public class ServiceMetadataMapper : Profile
    {
        public ServiceMetadataMapper()
        {
            CreateMap<ServiceMetadataDTO, ServiceMetadata>();

            CreateMap<ServiceMetadata, ServiceMetadataDTO>();
                
        }
    }
}
