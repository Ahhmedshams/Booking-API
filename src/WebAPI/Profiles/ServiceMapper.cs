using AutoMapper;

namespace WebAPI.Profiles
{
    public class ServiceMapper: Profile
    {
        public ServiceMapper() 
        {
            CreateMap<ServiceDTO, Service>();
            CreateMap<Service, ServiceDTO>();
        }
        
    }
}
