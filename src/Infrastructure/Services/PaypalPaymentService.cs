using Application.Common.Interfaces.Services;
using AutoMapper;
using Infrastructure.Utility.Extensions;
using Microsoft.Extensions.Configuration;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class PaypalPaymentService : IPaymentService
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public PaypalPaymentService(IConfiguration configuration, IMapper mapper)
        {
            this.configuration=configuration;
            this.mapper=mapper;
        }
        

        public async Task<string> MakePayment(IBookingItemRepo bookingItemRepo, decimal amount, int bookingID)
        {


            Dictionary<string, string> _config = new Dictionary<string, string>();
            _config["mode"] = configuration["Paypal:Mode"];
            _config["clientId"] = configuration["Paypal:PublicKey"];
            _config["clientSecret"] = configuration["Paypal:SecretKey"];

            var token = new OAuthTokenCredential(_config).GetAccessToken();


            APIContext paypalApi = new APIContext(token) { Config = _config };


            var bookingItems = bookingItemRepo.GetAllBooksItemsByBookingId(bookingID);

            Payment createdPayment = null;
            try
            {
                var payment = new Payment()
                {
                    intent = "sale",
                    payer = new Payer() { payment_method = "paypal" },
                    transactions = new List<Transaction>() {
                        new Transaction() {
                            amount = new() { currency = "USD", total = bookingItems.Sum(b => b.Price).ToString() },
                            item_list =  bookingItems.ToPaypalItemsList()
                        } 
                    },
                    
                    redirect_urls = new RedirectUrls()
                    {
                        cancel_url = configuration["Paypal:CancelUrl"],
                        return_url = $"{configuration["Paypal:SuccessUrl"]}?bookingID={bookingID}&userID={bookingItems[0].ClientBooking.UserId}"
                    }
                  
                };

                createdPayment =  payment.Create(paypalApi);
            }
            catch (Exception e)
            {
                throw e;
            }
           
            if (createdPayment != null)
            {
                foreach (var link in createdPayment.links)
                {
                    if (link.rel.Equals("approval_url"))
                    {
                        return link.href;
                    }
                }
            }
           

            throw new Exception("cannot create payapal session");
        }

        public Task<bool> RefundPayment(string paymentID)
        {
            throw new NotImplementedException();
        }
    }
}
