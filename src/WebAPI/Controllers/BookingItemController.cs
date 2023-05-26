using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingItemController : BaseController
    {
        private readonly IBookingItemRepo bookingItemRepo;
        private readonly IMapper mapper;

        public BookingItemController(IBookingItemRepo _bookingItemRepo,
                                    IMapper _mapper) 
        {
            bookingItemRepo = _bookingItemRepo;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookingItems = await bookingItemRepo.GetAllAsync(true, b => b.ClientBooking, r => r.Resource);
            if (bookingItems.Count() == 0)
                return CustomResult("No Booking Items Found", HttpStatusCode.NotFound);
            var bookingItemsDTO = mapper.Map<IEnumerable<BookingItem>, IEnumerable<BookingItemResDTO>>(bookingItems);
            return CustomResult(bookingItemsDTO);
        }

        [HttpGet("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> GetById(int bookingId, int resourceId)
        {
            var bookingItem = await bookingItemRepo.GetBookByIdAsync(bookingId, resourceId , b=>b.ClientBooking,r => r.Resource);
            if (bookingItem == null)
                return CustomResult($"No Booking Item Found For booking Id: {bookingId} with resource Id: {resourceId} ",
                    HttpStatusCode.NotFound);

            var clientBookingDTO = mapper.Map<BookingItem, BookingItemResDTO>(bookingItem);
            return CustomResult(clientBookingDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(BookingItemReqDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            var bookingItem = mapper.Map<BookingItemReqDTO, BookingItem>(bookingItemDTO);
            await bookingItemRepo.AddAsync(bookingItem);
            return CustomResult(bookingItem);
        }

        //[HttpPut("{bookingId:int}/{resourceId:int}")]
        //public async Task<IActionResult> Edit(int bookingId, int resourceId,
        //                                    BookingItemReqDTO bookingItemDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return CustomResult(ModelState, HttpStatusCode.BadRequest);
        //    var bookingItem = mapper.Map<BookingItemReqDTO, BookingItem>(bookingItemDTO);
        //    await bookingItemRepo.EditBookAsyn(bookingId, resourceId, bookingItem);
        //    return CustomResult(bookingItem);
        //}

        [HttpDelete("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> Delete(int bookingId, int resourceId)
        {
            var service = await GetById(bookingId, resourceId);
            if (service == null)
                return CustomResult($"No Booking Item For booking Id: {bookingId} with resource Id: {resourceId} ",
                    HttpStatusCode.NotFound);
            await bookingItemRepo.DeleteBookAsyn(bookingId, resourceId);
            return CustomResult(HttpStatusCode.NoContent);
        }
    }
}
