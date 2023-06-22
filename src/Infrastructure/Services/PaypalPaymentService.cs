using Application.Common.Interfaces.Services;
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

        public PaypalPaymentService(IConfiguration configuration)
        {
            this.configuration=configuration;
        }
        public Task<bool> CancelPayment(string paymentID)
        {
            throw new NotImplementedException();
        }

        public async Task<string> MakePayment(IBookingItemRepo bookingItemRepo, decimal amount, int bookingID)
        {

            Dictionary<string, string> _config = new Dictionary<string, string>();
            _config["mode"] = configuration["Paypal:Mode"];
            _config["clientId"] = configuration["Paypal:PublicKey"];
            _config["clientSecret"] = configuration["Paypal:SecretKey"];

            var token = new OAuthTokenCredential(_config).GetAccessToken();


            APIContext paypalApi = new APIContext(token) { Config = _config };

            Payment createdPayment = null;
            try
            {
                var payment = new Payment()
                {
                    intent = "sale",
                    payer = new Payer() { payment_method = "paypal"},
                    transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            amount = new Amount(){ currency = "USD", total = "100"},
                            description = "resource test",

                         
                        }
                    },
                    redirect_urls = new RedirectUrls()
                    {
                        cancel_url = configuration["Paypal:CancelUrl"],
                        return_url = configuration["Paypal:SuccessUrl"]
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
    }
}
