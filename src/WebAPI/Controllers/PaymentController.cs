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
        private readonly IBookingItemRepo bookingItemRepo;

        public PaymentController(IPaymentService paymentService, IBookingItemRepo bookingItemRepo)
        {
            this.paymentService=paymentService;
            this.bookingItemRepo=bookingItemRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var bookingItemSpec = new BookingItemSpecParams() { BookId = 3 };
           // var bookingItems = await bookingItemRepo.GetAllBooksItemsByBookingId(3);


            var paymentUrl = paymentService.MakePayment(bookingItemRepo, 400, 3);

            return CustomResult("created", paymentUrl,System.Net.HttpStatusCode.Created);
        }
    }
}
