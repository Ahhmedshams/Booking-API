using AutoMapper;

namespace WebAPI.Profiles
{
    public class PricesBookingReportMapper: Profile
    {
        public PricesBookingReportMapper() 
        {
            CreateMap<ClientBooking, PricesBookingReportDTO>()
                .ForMember(h => h.Price, o => o.MapFrom(b => b.TotalCost));

            CreateMap<PricesBookingReportDTO, ClientBooking>()
                .ForMember(b => b.TotalCost , o => o.MapFrom(h =>  h.Price));
                
        }

    }
}
