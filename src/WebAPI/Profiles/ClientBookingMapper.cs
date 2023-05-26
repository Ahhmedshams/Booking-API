using AutoMapper;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Profiles
{
    public class ClientBookingMapper : Profile
    {
        private readonly IAsyncRepository<Service> serviceRepo;
        public ClientBookingMapper(IAsyncRepository<Service> serviceRepo)
        {
            this.serviceRepo = serviceRepo;
        }
        public ClientBookingMapper()
        {

            CreateMap<ClientBookingReqDTO, ClientBooking>()
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.Service))
                .ForMember(dest => dest.Service, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.Service != null)
                    {
                        dest.Service = serviceRepo.GetByIdAsync(src.Service).Result;
                    }
                });

            CreateMap<ClientBooking, ClientBookingDTO>()
                .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service.Name));
            
        }


       
    }


}
