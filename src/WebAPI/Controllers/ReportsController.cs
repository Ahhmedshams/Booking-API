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
        public async Task<IActionResult> TotalPriceReport(DateTime startDate , DateTime endDate, int serviceId)
        {
            var totalPrice = await _clientBookingRepo.PriceReport(startDate, endDate, serviceId); 
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

        [HttpGet("TopResources")]
        public async Task<IActionResult> TopResourcesReport(DateTime startDate, DateTime endDate, int number)
        {
            var topResources = await _bookingItemRepo.TopResourcesReport(startDate, endDate, number);
            return CustomResult(topResources);
        }

        [HttpGet("ResTypeBookingsNo")]
        public async Task<IActionResult> ResourceTypeBookingsReport(DateTime startDate, DateTime endDate)
        {
            var resTypeBookingsNo = await _bookingItemRepo.ResourceTypeBookingsReport(startDate, endDate);
            return CustomResult(resTypeBookingsNo);
        }

        [HttpGet("ResTypeSoldPerMonth")]
        public async Task<IActionResult> ResTypeSoldPerMonthReport(DateTime startDate , DateTime endDate)
        {
            var resTypeSoldPerMonth = await _bookingItemRepo.ResTypeSoldPerMonthReport(startDate, endDate);
            return CustomResult(resTypeSoldPerMonth);
        }

        [HttpGet("CustomerNo")]
        public async Task<IActionResult> CustomerReport(DateTime startDate, DateTime endDate)
        {
            var newCustomerNo = await _userRepo.UserReport(startDate, endDate);
            return CustomResult(new {NewCustomerNo = newCustomerNo});
        }

    }
}
