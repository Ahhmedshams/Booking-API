using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using CoreApiResponse;
using Infrastructure.Persistence.Specification.BookingItemSpec;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly IPayemntTransactionRepository payemntTransactionRepository;
        private readonly IBookingItemRepo bookingItemRepo;

        public PaymentController(IPaymentService paymentService, IPayemntTransactionRepository payemntTransactionRepository, IBookingItemRepo bookingItemRepo)
        {
            this.paymentService=paymentService;
            this.payemntTransactionRepository=payemntTransactionRepository;
            this.bookingItemRepo=bookingItemRepo;
        }


        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
           


            var paymentUrl = paymentService.MakePayment(bookingItemRepo, 400, 2);

            return CustomResult("created", paymentUrl,System.Net.HttpStatusCode.Created);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> Refund(int bookingID)
        {

            var payments = await payemntTransactionRepository.FindAsync(p => p.ClientBookingId == bookingID);

            //TODO: check if pyament is confirmed and paid
            
            string paymentID = payments.FirstOrDefault()?.TransactionId;

            var refund = paymentService.CancelPayment(paymentID);

            // TODO: change payment transaction status to refunded

            return CustomResult("created", refund, System.Net.HttpStatusCode.Created);
        }
    }
}
