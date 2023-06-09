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
        [HttpGet("HighestPrice")]
        public async Task<IActionResult> TopPricesReport(DateTime startDate , DateTime endDate)
        {
            var pricingReport = await _clientBookingRepo.GetPriceReport(startDate, endDate); 
            if(pricingReport.Count() == 0)
                return CustomResult("No Clint's Book Found in this range of date", HttpStatusCode.NotFound);

            var pricingReportDTO = _mapper.Map<IEnumerable<ClientBooking>, 
                                            IEnumerable<PricesBookingReportDTO>>(pricingReport);
            return CustomResult(pricingReportDTO);
        }

        [HttpGet("Booking")]
        public async Task<IActionResult> BookingReport(DateTime startDate, DateTime endDate)
        {
            var bookingReport =  await _clientBookingRepo.GetBookingReport(startDate,endDate);
            if(bookingReport.Count() == 0)
                return CustomResult("No Clint's Book Found in this range of date", HttpStatusCode.NotFound);

            var bookingReportDTO = _mapper.Map<IEnumerable<ClientBooking>,
                                    IEnumerable<ClientBookingDTO>>(bookingReport);
            return CustomResult(bookingReportDTO);

        }

        [HttpGet("Resource")]
        public async Task<IActionResult> MostUsedResource(DateTime startDate, DateTime endDate)
        {
            var mostUsedResourceReport = await _bookingItemRepo.GetMostUsedResourcesReport(startDate, endDate);
            if(mostUsedResourceReport.Count() == 0)
                return CustomResult("No resources used in this range of date", HttpStatusCode.NotFound);
            return CustomResult(mostUsedResourceReport);
        }

        [HttpGet("NewCustomer")]
        public async Task<IActionResult> NewCustomerReport(DateTime startDate, DateTime endDate)
        {
            var newCustomerReport = await _userRepo.UserReport(startDate, endDate);
            if (newCustomerReport.Count() == 0)
                return CustomResult("No Customer registerd in this range of date" , HttpStatusCode.NotFound);

            var newCustomerReportDTO = _mapper.Map<IEnumerable<ApplicationUser>,
                                    IEnumerable<RegisterUserDto>>(newCustomerReport);
            return CustomResult(newCustomerReportDTO);
        }

    }
}
