using AutoMapper;

namespace WebAPI.Profiles
{
    public class ServiceMapper: Profile
    {
        public ServiceMapper() 
        {
            CreateMap<ServiceDTO, Service>().ReverseMap();
            CreateMap<Service,ServiceRegionDTO>().ReverseMap(); 

            //CreateMap<Service, ServiceResDTO>()
            //    .ForMember(dest => dest.ResoureceTypes, 
            //                opt => opt.MapFrom(src => 
            //                src.Metadata.Select(m => m.ResourceType.Name).ToList()));

        }

    }
}
