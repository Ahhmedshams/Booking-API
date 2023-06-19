using Application.Common.Interfaces.Services;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Infrastructure.Persistence.Specification.BookingItemSpec;
using Infrastructure.Persistence.Repositories;

using AutoMapper;
using Infrastructure.Utility.Extensions;

namespace Infrastructure.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly IConfiguration configuration;

        public StripePaymentService(IConfiguration configuration)
        {
            this.configuration=configuration;
        }


        public Task<bool> CancelPayment(int bookingID)
        {
            throw new NotImplementedException();
        }

        public async Task<string> MakePayment(IBookingItemRepo bookingItemRepo, decimal amount, int bookingID)
        {
         
            // TODO: read all showable items, sum all material in one item, sum all items like cars in one item.
            var bookingItems = bookingItemRepo.GetAllBooksItemsByBookingId(bookingID);

            var sessionLineItemOptions = bookingItems.ToSessionLineItemOptionsObject();

            var options = new SessionCreateOptions
            {
                SuccessUrl = configuration["Stripe:SuccessUrl"],
                LineItems = sessionLineItemOptions,             
                Mode = "payment",
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            };
            var service = new SessionService();
            
            var session = service.Create(options);

            return await Task.FromResult(session.Url);
        }
    }
}
