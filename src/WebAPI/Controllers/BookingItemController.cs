using AutoMapper;
using CoreApiResponse;
using Infrastructure.Persistence.Specification.BookingItemSpec;
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
        public async Task<IActionResult> GetAll([FromQuery] BookingItemSpecParams specParams)
        {
            var spec = new BookingItemSpecification(specParams);
            var bookingItems = await bookingItemRepo.GetAllBooksWithSpec(spec);
            var bookingItemsDTO = mapper.Map<IEnumerable<BookingItem>, IEnumerable<BookingItemDTO>>(bookingItems);
            return CustomResult(bookingItemsDTO);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromQuery] int bookingId,[FromQuery] int resourceId, BookingItemDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            return await ProcessActionOne(bookingItemDTO, async () =>
            {
                var bookingItem = mapper.Map<BookingItemDTO, BookingItem>(bookingItemDTO);
                await bookingItemRepo.EditBookAsyn(bookingId, resourceId, bookingItem);
                return bookingItemDTO;
            });
        }

        [HttpDelete("DeleteOne")]
        public async Task<IActionResult> Delete([FromQuery] int bookingId,[FromQuery] int resourceId)
        {
            var service = await bookingItemRepo.GetBookByComplexIdsAsync(bookingId, resourceId);
            if (service == null)
                return CustomResult($"No Booking Items For bookId: {bookingId} with resourceId: {resourceId} ",
                    HttpStatusCode.NotFound);

            await bookingItemRepo.DeleteBookAsyn(bookingId, resourceId);
            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpDelete("DeleteBulk")]
        public async Task<IActionResult> DeleteBulk([FromQuery] int bookingId)
        {
            var bookingItems = await bookingItemRepo.GetBookItemByIdAsync(bookingId);
            if (bookingItems == null)
                return CustomResult($"No Booking Items Found For BookId: {bookingId} ",
                                    HttpStatusCode.NotFound);

            await bookingItemRepo.DeleteBulk(bookingId);
            return CustomResult(HttpStatusCode.NoContent);
        }


        private async Task<IActionResult> ProcessActionOne(BookingItemDTO bookingItemDTO, Func<Task<BookingItemDTO>> action)
        {
            bool existenceOfClientBook = await bookingItemRepo.IsClientBookExis(bookingItemDTO.BookingId);
            if(! existenceOfClientBook)
                return CustomResult($"No Client Book found for Id: {bookingItemDTO.BookingId}", HttpStatusCode.BadRequest);

            bool existenceofResource = await bookingItemRepo.IsResourecExist(bookingItemDTO.ResourceId);
            if (!existenceofResource)
                return CustomResult($"No Resource found for Id: {bookingItemDTO.ResourceId}", HttpStatusCode.BadRequest);


            bool checkDuplicate = await bookingItemRepo.CheckDuplicateKey(bookingItemDTO.BookingId, bookingItemDTO.ResourceId);
            if (checkDuplicate)
                return CustomResult("Duplicate key violation.", HttpStatusCode.BadRequest);

            var result = await action.Invoke();
            return CustomResult(result);
        }


	}
}
