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

        public WebhookController(IConfiguration configuration, IPayemntTransactionRepository payemntTransactionRepository)
        {
            this.configuration=configuration;
            this.payemntTransactionRepository=payemntTransactionRepository;
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                //var stripeEvent = EventUtility.ParseEvent(json);

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
                            TransactionId = session.Id,
                            PaymentMethodId = 1
                        ,
                            Status = PaymentStatus.Successful
                        };

                        await payemntTransactionRepository.AddAsync(payementTransaction);


                        break;
                   // case Events.CustomerSourceExpiring:
                        //send reminder email to update payment method
                  //      break;
                 //   case Events.ChargeFailed:
                        //do something
                   //     break;
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }


        }
    }
}
