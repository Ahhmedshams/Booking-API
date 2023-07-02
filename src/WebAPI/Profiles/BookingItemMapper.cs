using AutoMapper;

namespace WebAPI.Profiles
{
   
    public class BookingItemMapper : Profile
    {
        public BookingItemMapper()
        {
            CreateMap<BookingItemDTO, BookingItem>();
            CreateMap<BookingItem, BookingItemDTO>();
                
        }
    }
}
