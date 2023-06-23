using CoreApiResponse;
using Domain.Enums;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPal.Api;
using Stripe;
using Stripe.Checkout;
using System.Net;

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


        [HttpPost("paypal")]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> HandlePaypalWebhook()
        {

            Console.WriteLine("\n\n\npayment created\n\n");

            var json = await new StreamReader(HttpContext.Request.Body)
                .ReadToEndAsync();

            var headersDict = HttpContext.Request.Headers;

            // verify event

            var webhookEvent = JsonConvert.DeserializeObject<WebhookEvent>(json);

            switch (webhookEvent.event_type)
            {
                case "PAYMENT.SALE.COMPLETED":
                    Console.WriteLine("\n*************************\n\n");

                    Console.WriteLine("\n\nPAYMENT.SALE.COMPLETED\n\n");
                    Console.WriteLine("\n*************************\n\n");


                    break;
                case "PAYMENT.SALE.PENDING":
                    Console.WriteLine("\n*************************\n\n");

                    Console.WriteLine("\n\nPAYMENT.SALE.PENDING\n\n");
                    Console.WriteLine("\n*************************\n\n");
                    break;
                default:
                    Console.WriteLine("\n*************************\n\n");

                    Console.WriteLine("\n\nno event selected\n\n");
                    Console.WriteLine("\n*************************\n\n");

                    break;
            }


            return CustomResult("Success", "Success");
        }

        [HttpGet("paypal/success")]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> PaypalSuccess(string PaymentId, string PayerID, int bookingID)
        {

            Dictionary<string, string> _config = new Dictionary<string, string>();
            _config["mode"] = configuration["Paypal:Mode"];
            _config["clientId"] = configuration["Paypal:PublicKey"];
            _config["clientSecret"] = configuration["Paypal:SecretKey"];

            var token = new OAuthTokenCredential(_config).GetAccessToken();


            APIContext paypalApi = new APIContext(token) { Config = _config };

            var paymentExecution = new PaymentExecution() { payer_id = PayerID };

            var payment = new Payment() { id = PaymentId };

            
            try
            {
                Payment executedPayment = executedPayment = payment.Execute(paypalApi, paymentExecution);
                if (executedPayment.state == "approved")
                {
                    return Redirect(configuration["Paypal:SuccessUrl"]);
                }

            }
            catch (Exception e)
            {
                // "Transaction is declined due to compliance violation."
            }

            return CustomResult("failed");
        }
        

        

    }
}
