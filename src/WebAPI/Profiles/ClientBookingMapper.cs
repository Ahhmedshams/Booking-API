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
             

        }



    }


}
