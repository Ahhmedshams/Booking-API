using AutoMapper;
using CoreApiResponse;
using Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : BaseController
    {
        private readonly IClientBookingRepo _clientBookingRepo;
        private readonly IApplicationUserRepo _userRepo;
        private readonly IBookingItemRepo _bookingItemRepo;
        private readonly IMapper _mapper;

        public ReportsController(IClientBookingRepo clientBookingRepo,
                                 IApplicationUserRepo userRepo,
                                 IBookingItemRepo bookingItemRepo,
                                 IMapper mapper)
        {
            _clientBookingRepo = clientBookingRepo;
            _userRepo = userRepo;
            _bookingItemRepo = bookingItemRepo;
            _mapper = mapper;
        }
        [HttpGet("TotalPrice")]
        public async Task<IActionResult> TopPricesReport(DateTime startDate , DateTime endDate)
        {
            var totalPrice = await _clientBookingRepo.PriceReport(startDate, endDate); 
            return CustomResult(new {TotalPrice = totalPrice });
        }

        [HttpGet("CancelledBookings")]
        public async Task<IActionResult> CancelledBookingsReport(DateTime startDate, DateTime endDate)
        {
            var canceledBookingsNo = await _clientBookingRepo.CancelledBookingsReport(startDate, endDate);
            return CustomResult(new {CanceledBookingsNo = canceledBookingsNo});
        }

        [HttpGet("BookingsNo")]
        public async Task<IActionResult> BookingsNoReport(DateTime startDate, DateTime endDate)
        {
            var bookingsNo =  await _clientBookingRepo.BookingsNoReport(startDate,endDate);
            return CustomResult(new {BookingsNo = bookingsNo});

        }

        [HttpGet("Top5Resources")]
        public async Task<IActionResult> Top5ResourcesReport(DateTime startDate, DateTime endDate)
        {
            var mostUsedResourceReport = await _bookingItemRepo.Top5ResourcesReport(startDate, endDate);
            return CustomResult(mostUsedResourceReport);
        }

        [HttpGet("NewCustomerNo")]
        public async Task<IActionResult> NewCustomerReport(DateTime startDate, DateTime endDate)
        {
            var newCustomerNo = await _userRepo.UserReport(startDate, endDate);
            return CustomResult(new {NewCustomerNo = newCustomerNo});
        }

    }
}
