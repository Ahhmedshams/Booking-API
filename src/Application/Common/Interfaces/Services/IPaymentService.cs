using Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface IPaymentService
    {
        public Task<string> MakePayment(IBookingItemRepo bookingItemRepo,decimal amount, int bookingID);

        //public Task<bool> CancelPayment(string paymentID);

        public Task<bool> RefundPayment(string paymentID);


     //   public Task SetCustomerInfo(string name, string email, string address);


        //   public void SetCardInfo(string id, string expireDate, int ccv);
    }
}
