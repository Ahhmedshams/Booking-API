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
        private readonly IAsyncRepository<ClientBooking> clientBookingRepo;
        private readonly IResourceRepo resourceRepo;

        public BookingItemController(IBookingItemRepo _bookingItemRepo,
                                    IMapper _mapper,
                                    IAsyncRepository<ClientBooking> _clientBookingRepo,
                                    IResourceRepo _resourceRepo) 
        {
            bookingItemRepo = _bookingItemRepo;
            mapper = _mapper;
            clientBookingRepo = _clientBookingRepo;
            resourceRepo = _resourceRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookingItems = await bookingItemRepo.GetAllAsync(true, b => b.ClientBooking, r => r.Resource);
            if (bookingItems.Count() == 0)
                return CustomResult("No Booking Items Found", HttpStatusCode.NotFound);
            var bookingItemsDTO = mapper.Map<IEnumerable<BookingItem>, IEnumerable<BookingItemDTO>>(bookingItems);
            return CustomResult(bookingItemsDTO);
        }

        [HttpGet("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> GetById(int bookingId, int resourceId)
        {
            var bookingItem = await bookingItemRepo.GetBookByIdAsync(bookingId, resourceId , b=>b.ClientBooking,r => r.Resource);
            if (bookingItem == null)
                return CustomResult($"No Booking Item Found For booking Id: {bookingId} with resource Id: {resourceId} ",
                    HttpStatusCode.NotFound);

            var clientBookingDTO = mapper.Map<BookingItem, BookingItemDTO>(bookingItem);
            return CustomResult(clientBookingDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(BookingItemDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessAction(bookingItemDTO, async () =>
            {
                var bookingItem = mapper.Map<BookingItemDTO, BookingItem>(bookingItemDTO);
                await bookingItemRepo.AddAsync(bookingItem);
                return bookingItemDTO;
            });
        }

        [HttpPut("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> Edit(int bookingId, int resourceId, BookingItemDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessAction(bookingItemDTO, async () =>
            {
                var bookingItem = mapper.Map<BookingItemDTO, BookingItem>(bookingItemDTO);
                await bookingItemRepo.EditBookAsyn(bookingId, resourceId, bookingItem);
                return bookingItemDTO;
            });
        }

        private async Task<IActionResult> ProcessAction(BookingItemDTO bookingItemDTO, Func<Task<BookingItemDTO>> action)
        {
            int existenceofClientBookAndRes = await bookingItemRepo.CheckExistenceOfBookIdAndResId(bookingItemDTO.BookingId, bookingItemDTO.ResourceId);
            switch (existenceofClientBookAndRes)
            {
                case 1:
                    return CustomResult($"No Client Book found for Id: {bookingItemDTO.BookingId}");
                case -1:
                    return CustomResult($"No Resource found for Id: {bookingItemDTO.ResourceId}");
            }

            bool checkDuplicate = await bookingItemRepo.CheckDuplicateKey(bookingItemDTO.BookingId, bookingItemDTO.ResourceId);
            if (checkDuplicate)
                return CustomResult("Duplicate key violation. The specified key already exists in the system");

            var result = await action.Invoke();
            return CustomResult(result);
        }

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
