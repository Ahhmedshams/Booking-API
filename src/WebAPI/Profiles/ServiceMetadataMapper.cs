using AutoMapper;

namespace WebAPI.Profiles
{
    public class ServiceMetadataMapper : Profile
    {
        public ServiceMetadataMapper()
        {
            CreateMap<ServiceMetadataReqDTO, ServiceMetadata>();

            CreateMap<ServiceMetadata, ServiceMetadataResDTO>()
                .ForMember(des => des.ServiceName, obj => obj.MapFrom(src => src.Service.Name))
                .ForMember(des => des.ResourceTypeName, obj => obj.MapFrom(src => src.ResourceType.Name));
        }
    }
}
