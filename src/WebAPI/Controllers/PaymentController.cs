using Application.Common.Interfaces.Services;
using CoreApiResponse;
using Domain.Enums;
using Infrastructure.Factories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly PaymentFactory paymentFactory;
        private readonly IPayemntTransactionRepository payemntTransactionRepository;
        private readonly IClientBookingRepo clientBookingRepo;
        private readonly IBookingItemRepo bookingItemRepo;

        public PaymentController(PaymentFactory paymentFactory, IPayemntTransactionRepository payemntTransactionRepository, IClientBookingRepo clientBookingRepo ,IBookingItemRepo bookingItemRepo)
        {
            this.paymentFactory=paymentFactory;
            this.payemntTransactionRepository=payemntTransactionRepository;
            this.clientBookingRepo=clientBookingRepo;
            this.bookingItemRepo=bookingItemRepo;
        }


        [HttpPost("checkout/{bookingID:int}")]
        public async Task<IActionResult> Checkout(int bookingID)
        {
            // TODO: check if a user already has a payment session to this booking.

            var booking = await clientBookingRepo.GetBookingById(bookingID);

            if (booking == null)
                return CustomResult("There is no booking with this id", HttpStatusCode.NotFound);

            if (booking.Status != BookingStatus.Pending)
                return CustomResult("This request is invalid.", HttpStatusCode.BadRequest);

            string paymentType = ((PaymentMethodType)booking.paymentTransaction.PaymentMethodId).ToString();

            IPaymentService service = paymentFactory.CreatePaymentService(paymentType);


            var paymentUrl = await service.MakePayment(payemntTransactionRepository,bookingItemRepo, booking.TotalCost, bookingID);

            return CustomResult("created", new { Result= paymentUrl }, HttpStatusCode.Created);
        }

        [HttpPost("refund/{bookingID:int}")]
        public async Task<IActionResult> Refund(int bookingID)
        {


            var booking = await clientBookingRepo.GetBookingById(bookingID);

            if (booking.Status != BookingStatus.Confirmed)
                return CustomResult("Booking is not paid.", HttpStatusCode.BadRequest);

       

            string paymentType = ((PaymentMethodType)booking.paymentTransaction.PaymentMethodId).ToString();


            IPaymentService service = paymentFactory.CreatePaymentService(paymentType);
            var refund = await service.RefundPayment(booking.paymentTransaction.TransactionId);
             
            // TODO: handle all tranactions atomic execution.
            if (refund)
            {
                await payemntTransactionRepository.Refund(booking.paymentTransaction.Id);
                return CustomResult("Booking successfully refunded", refund, HttpStatusCode.Created);

            }

            return CustomResult("failed to refund", refund, HttpStatusCode.BadRequest);

        }
    }
}
