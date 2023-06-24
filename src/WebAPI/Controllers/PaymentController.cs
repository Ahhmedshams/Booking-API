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
        public async Task<IActionResult> Checkout(string paymentType, int bookingID)
        {
            // TODO: check if a user already has a payment session to this booking.

            var booking = await clientBookingRepo.GetBookingById(bookingID);

            if (booking == null)
                return CustomResult("There is no booking with this id", System.Net.HttpStatusCode.NotFound);


            bool isValidPaymentType = false;
            if (paymentType != null)
            {
                foreach (string method in Enum.GetNames(typeof(PaymentMethodType)))
                {
                    if (method.ToLower() == paymentType.ToLower())
                    {
                        isValidPaymentType = true;
                        break;
                    }
                }
            }


            if (!isValidPaymentType)
                return CustomResult("Invalid payment method", HttpStatusCode.BadRequest);


            IPaymentService service = paymentFactory.CreatePaymentService(paymentType);


            var paymentUrl = service.MakePayment(bookingItemRepo, booking.TotalCost, bookingID);

            return CustomResult("created", paymentUrl,System.Net.HttpStatusCode.Created);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> Refund(int bookingID)
        {
            var booking = await clientBookingRepo.GetBookingById(bookingID);

            if (booking.Status != BookingStatus.Confirmed)
                return CustomResult("Booking is not paid.", System.Net.HttpStatusCode.BadRequest);

            var payments = await payemntTransactionRepository.FindAsync(p => p.ClientBookingId == bookingID);
            var paymentTransaction = payments.FirstOrDefault();

            IPaymentService service = paymentFactory.CreatePaymentService("card");
            var refund = await service.RefundPayment(paymentTransaction.TransactionId);

            // TODO: handle all tranactions atomic execution.
            if (refund)
            {
                await payemntTransactionRepository.Refund(paymentTransaction.Id);
                return CustomResult("Booking successfully refunded", refund, System.Net.HttpStatusCode.Created);

            }

            return CustomResult("failed to refund", refund, System.Net.HttpStatusCode.BadRequest);

        }
    }
}
