using Application.Common.Interfaces.Services;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories
{
    public class PaymentFactory
    {
        private readonly IServiceProvider serviceProvider;

        public PaymentFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider=serviceProvider;
        }

        public IPaymentService CreatePaymentService(string paymentType)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                switch (paymentType.ToLower())
                {
                    case "card":
                        return (IPaymentService)scope.ServiceProvider.GetRequiredService<StripePaymentService>();
                    case "paypal":
                        return (IPaymentService)scope.ServiceProvider.GetRequiredService<PaypalPaymentService>();                       
                    default:
                        throw new ArgumentException("Invalid payment type");
                        
                }

            }

   
            return null;
        }
    }
}
