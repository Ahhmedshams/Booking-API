using AutoMapper;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Profiles
{
    public class ClientBookingMapper : Profile
    {
        public ClientBookingMapper()
        {
            CreateMap<ClientBookingDTO, ClientBooking>();

            CreateMap<ClientBooking, ClientBookingDTO>()
                .ForMember(cb => cb.UserEmail, o => o.MapFrom(c => c.User.Email))
                .ForMember(cb => cb.ServiceName, o => o.MapFrom(c => c.Service.Name));

        }



    }


}
