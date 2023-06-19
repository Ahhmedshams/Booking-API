using AutoMapper;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Profiles
{
    public class BookingItemMapper: Profile
    {
        public BookingItemMapper() {
            CreateMap<BookingItem, SessionLineItemOptions>()
                .ForMember(dest => dest.PriceData, opt => opt.MapFrom(src => 
                new SessionLineItemPriceDataOptions 
                { Currency = "egp", UnitAmountDecimal = src.Price, ProductData = new() { Name = "Name" } }))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => 1));
                 //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                 //.ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseName))
                 //.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                 //.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Du
        }
    }
}
