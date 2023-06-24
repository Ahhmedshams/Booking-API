using AutoMapper;

namespace WebAPI.Profiles
{
    public class TicketProfile:Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket,TicketDTO>().ReverseMap();
        }
    }
}
