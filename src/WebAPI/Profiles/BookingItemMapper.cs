using AutoMapper;

namespace WebAPI.Profiles
{
   
    public class BookingItemMapper : Profile
    {
        public BookingItemMapper()
        {
            CreateMap<BookingItemReqDTO, BookingItem>();
            CreateMap<BookingItem, BookingItemResDTO>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.ResourceType.Name));
        }
    }
}
