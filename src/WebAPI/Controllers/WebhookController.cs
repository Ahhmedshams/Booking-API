using CoreApiResponse;
using Domain.Enums;
using Infrastructure.Persistence.Repositories;
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
        private readonly ILogger logger;
        private readonly IPayemntTransactionRepository payemntTransactionRepository;

        public WebhookController(IConfiguration configuration, ILogger logger, IPayemntTransactionRepository payemntTransactionRepository)
        {
            this.configuration=configuration;
            this.logger=logger;
            this.payemntTransactionRepository=payemntTransactionRepository;
        }


        [HttpPost]
        public async Task<IActionResult> Index()
        {

            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                //var stripeEvent = EventUtility.ParseEvent(json);

                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], configuration["Stripe:WebhookSigningKey"]);

                var session = (Session) stripeEvent.Data.Object;
                var metadata = session.Metadata;


                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        logger.LogInformation("Payment succeeded");

                        //stripeEvent.Data.Object

                        var payementTransaction = new PaymentTransaction() { ClientBookingId = int.Parse(metadata["bookingID"]),
                        Amount = (decimal) session.AmountTotal, UserId = session.ClientReferenceId, TransactionId = session.Id, PaymentMethodId = 1
                        , Status = PaymentStatus.Successful
                        };

                      await payemntTransactionRepository.AddAsync(payementTransaction);
                        

                        break;
                    case Events.CustomerSourceExpiring:
                        //send reminder email to update payment method
                        break;
                    case Events.ChargeFailed:
                        //do something
                        break;
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
