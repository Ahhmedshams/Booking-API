using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Infrastructure.Persistence.Specification.ClientBookingSpec;
using Application.Common.Interfaces.Services;
using WebAPI.Utility;
using Infrastructure.Factories;
using Domain.Enums;
using Infrastructure.Persistence.Repositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientBookingController : BaseController
    {
        private readonly IClientBookingRepo clientBookingRepo;
        private readonly IMapper mapper;
        private readonly PaymentFactory paymentFactory;
        private readonly IBookingItemRepo bookingItemRepo;
        private readonly IPayemntTransactionRepository paymentTransactionRepository;

        public ClientBookingController(IClientBookingRepo _clientBookingRepo,
                                        IMapper _mapper, PaymentFactory paymentFactory, 
                                        IBookingItemRepo bookingItemRepo, IPayemntTransactionRepository paymentTransactionRepository)
        {
            clientBookingRepo = _clientBookingRepo;
            mapper = _mapper;
            this.bookingItemRepo = bookingItemRepo;
            this.paymentTransactionRepository=paymentTransactionRepository;
            this.paymentFactory = paymentFactory;
            this.paymentFactory = paymentFactory;
            this.bookingItemRepo = bookingItemRepo;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ClientBookingSpecParam specParams)
        {
            var spec = new ClientBookingSpecification(specParams);
            var clientBooks = await clientBookingRepo.GetAllBookingsWithSpec(spec);

            var clientBooksDTO = mapper.Map<IEnumerable<ClientBooking>, IEnumerable<ClientBookingDTO>>(clientBooks);
            return CustomResult(clientBooksDTO);
        }


        [HttpGet("user/{id:Guid}")]
        public async Task<IActionResult> GetUserBooking(string id, [FromQuery] int? bookingId)
        {
            if (bookingId == null)
            {
                var result = await clientBookingRepo.GetUserBooking(id);
                if (result.Count() == 0)
                    return CustomResult($"No Client's Book found for this Id [ {id} ]", HttpStatusCode.NotFound);

                return CustomResult(result.ToClientBooking());
            }
            else
            {
                var result = await clientBookingRepo.GetUserBooking(id, (int)bookingId);
                if (result == null)
                    return CustomResult($"No Client's Book found for this Id [ {bookingId} ]", HttpStatusCode.NotFound);

                return CustomResult(result.ToClientBookingWithDetails());
            }

        }

        [HttpPost("CreateNewBooking")]
        public async Task<IActionResult> Add([FromBody] ClientBooking2DTO clientBooking2DTO, [FromQuery] string paymentType)
        {
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

            var bookingID = await clientBookingRepo.CreateNewBooking
                (clientBooking2DTO.UserID,
                clientBooking2DTO.Date,
                clientBooking2DTO.ServiceID,
                clientBooking2DTO.Location,
                clientBooking2DTO.StartTime,
                clientBooking2DTO.EndTime,
                clientBooking2DTO.ResourceIDs);

            if (bookingID == -1)
            {
                return CustomResult("Invalid data entered", HttpStatusCode.BadRequest);
            }

            var booking = await clientBookingRepo.GetByIdAsync(bookingID);


            IPaymentService service = paymentFactory.CreatePaymentService(paymentType);

            var paymentUrl = await service.MakePayment(paymentTransactionRepository, bookingItemRepo, booking.TotalCost, bookingID);


            return CustomResult("Payment session created successfully.", new { Result = paymentUrl }, HttpStatusCode.Created);
        }


        [HttpPut("CancelBooking/{bookingID:int}")]
        public async Task<IActionResult> CancelBooking(int bookingID)
        {

            var booking = await clientBookingRepo.GetBookingById(bookingID);

            if (booking == null)
                return CustomResult("There no booking with that id", HttpStatusCode.BadRequest);

            if (booking.Status != BookingStatus.Pending)
                return CustomResult("can't process this request", HttpStatusCode.BadRequest);

            await clientBookingRepo.CancelBooking(bookingID);


            return CustomResult("Succefully cancel booking");
        }

    }

}
