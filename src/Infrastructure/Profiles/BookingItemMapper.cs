using AutoMapper;
using PayPal.Api;
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


            CreateMap<BookingItem, Transaction>()
                //.ForMember(dest => dest.amount, opt => opt.MapFrom(src => new Amount() { total = src.Price.ToString(), currency = "USD"}))
                //.ForMember(dest => dest.amount, opt => opt.MapFrom(src => new Amount() { total = src.Price.ToString(), currency = "USD"}))

                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Resource.Name));

        }
    }
}
