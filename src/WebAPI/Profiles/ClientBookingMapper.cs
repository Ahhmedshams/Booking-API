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
            CreateMap<ClientBookingDTO, ClientBooking>();
            CreateMap<ClientBooking, ClientBookingDTO>();

        }



    }


}
