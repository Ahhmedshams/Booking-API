using CoreApiResponse;
using Domain.Enums;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IPayemntTransactionRepository payemntTransactionRepository;
        private readonly IBookingFlowRepo bookingFlowRepo;

        public WebhookController(IConfiguration configuration,
            IPayemntTransactionRepository payemntTransactionRepository,
            IBookingFlowRepo bookingFlowRepo)
        {
            this.configuration=configuration;
            this.payemntTransactionRepository=payemntTransactionRepository;
            this.bookingFlowRepo=bookingFlowRepo;
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], configuration["Stripe:WebhookSigningKey"], throwOnApiVersionMismatch: false);

                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        var session = (Session)stripeEvent.Data.Object;
                        var metadata = session.Metadata;
                        int bookingid = int.Parse(metadata["bookingID"]);

                        var payementTransaction = new PaymentTransaction()
                        {
                            ClientBookingId = bookingid,
                            Amount = (decimal)(session.AmountTotal/100.00),
                            UserId = session.ClientReferenceId,
                            TransactionId = session.PaymentIntentId
,
                            PaymentMethodId = 1
                        ,
                            Status = PaymentStatus.Successful
                        };

                        // TODO: Retry many times if there are faliure in saving - unit of work
                        await payemntTransactionRepository.AddAsync(payementTransaction);
                        bookingFlowRepo.ChangeStatusToConfirmed(bookingid);


                        break;
                    case Events.CheckoutSessionExpired:
                        break;

                    case Events.RefundCreated: break;


                   
                }
                return Ok();
            }
            catch (StripeException e)
            {
                //TODO: save error in database
                return BadRequest();
            }


        }
    }
}
