using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingItemController : ControllerBase
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
                return BadRequest("No Booking Items found");
            var bookingItemsDTO = mapper.Map<IEnumerable<BookingItem>, IEnumerable<BookingItemResDTO>>(bookingItems);
            return Ok(bookingItemsDTO);
        }

        [HttpGet("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> GetById(int bookingId, int resourceId)
        {
            var bookingItem = await bookingItemRepo.GetBookByIdAsync(bookingId, resourceId , b=>b.ClientBooking,r => r.Resource);
            if (bookingItem == null)
                return BadRequest($"No Booking Item found ");

            var clientBookingDTO = mapper.Map<BookingItem, BookingItemResDTO>(bookingItem);
            return Ok(clientBookingDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(BookingItemReqDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var bookingItem = mapper.Map<BookingItemReqDTO, BookingItem>(bookingItemDTO);
            await bookingItemRepo.AddAsync(bookingItem);
            return Ok(bookingItem);
        }

        [HttpPut("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> Edit(int bookingId, int resourceId,
                                            BookingItemReqDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var bookingItem = mapper.Map<BookingItemReqDTO, BookingItem>(bookingItemDTO);
            await bookingItemRepo.EditBookAsyn(bookingId, resourceId, bookingItem);
            return Ok(bookingItem);
        }

        [HttpDelete("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> Delete(int bookingId, int resourceId)
        {
            var service = GetById(bookingId, resourceId);
            if (service == null)
                return BadRequest("No Booking Item found to delete it");
            await bookingItemRepo.DeleteBookAsyn(bookingId, resourceId);
            return NoContent();
        }
    }
}
