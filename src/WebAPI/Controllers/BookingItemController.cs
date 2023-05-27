using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.DTO;

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
            var bookingItemsDTO = mapper.Map<IEnumerable<BookingItem>, IEnumerable<BookingItemDTO>>(bookingItems);
            return CustomResult(bookingItemsDTO);
        }

        [HttpGet("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> GetById( int bookingId, int resourceId)
        {
            var bookingItem = await bookingItemRepo.GetBookByComplexIdsAsync(bookingId, resourceId , b=>b.ClientBooking,r => r.Resource);
            if (bookingItem == null)
                return CustomResult($"No Booking Items Found For bookId: {bookingId} with resourceId: {resourceId} ",
                    HttpStatusCode.NotFound);

            var clientBookingDTO = mapper.Map<BookingItem, BookingItemDTO>(bookingItem);
            return CustomResult(clientBookingDTO);
        }

        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetByBookId(int bookingId)
        {
            var bookingItem = await bookingItemRepo.GetBookItemByIdAsync(bookingId, b => b.ClientBooking, r => r.Resource);
            if (bookingItem == null)
                return CustomResult($"No Booking Items Found For bookId: {bookingId} ",
                    HttpStatusCode.NotFound);

            var clientBookingDTO = mapper.Map<IEnumerable<BookingItem>, IEnumerable<BookingItemDTO>>(bookingItem);
            return CustomResult(clientBookingDTO);
        }


        [HttpPost("AddOne")]
        public async Task<IActionResult> AddOne(BookingItemDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessActionOne(bookingItemDTO, async () =>
            {
                var bookingItem = mapper.Map<BookingItemDTO, BookingItem>(bookingItemDTO);
                await bookingItemRepo.AddAsync(bookingItem);
                return bookingItemDTO;
            });
        }

        [HttpPost("AddBulk")]
        public async Task<IActionResult> AddBulk(IEnumerable<BookingItemDTO> bookingItemsDTOs)
        {
            if (bookingItemsDTOs.Count() == 0)
                return CustomResult("No booking items provided.");

            return await ProcessActionBulk(bookingItemsDTOs, async () =>
            {
                var addedItems = new List<BookingItemDTO>();

                foreach (var bookingItemDTO in bookingItemsDTOs)
                {
                    var bookingItem = mapper.Map<BookingItemDTO, BookingItem>(bookingItemDTO);
                    await bookingItemRepo.AddAsync(bookingItem);
                    addedItems.Add(bookingItemDTO);
                }

                return addedItems;
            });
        }

        [HttpPut("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> Edit(int bookingId, int resourceId, BookingItemDTO bookingItemDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessActionOne(bookingItemDTO, async () =>
            {
                var bookingItem = mapper.Map<BookingItemDTO, BookingItem>(bookingItemDTO);
                await bookingItemRepo.EditBookAsyn(bookingId, resourceId, bookingItem);
                return bookingItemDTO;
            });
        }

        [HttpDelete("{bookingId:int}/{resourceId:int}")]
        public async Task<IActionResult> Delete(int bookingId, int resourceId)
        {
            var service = await GetById(bookingId, resourceId);
            if (service == null)
                return CustomResult($"No Booking Items For bookId: {bookingId} with resourceId: {resourceId} ",
                    HttpStatusCode.NotFound);
            await bookingItemRepo.DeleteBookAsyn(bookingId, resourceId);
            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpDelete("{bookingId:int}")]
        public async Task<IActionResult> DeleteBulk(int bookingId)
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
                return CustomResult($"No Client Book found for Id: {bookingItemDTO.BookingId}");

            bool existenceofResource = await bookingItemRepo.IsResourecExist(bookingItemDTO.ResourceId);
            if (!existenceofResource)
                return CustomResult($"No Resource found for Id: {bookingItemDTO.ResourceId}");


            bool checkDuplicate = await bookingItemRepo.CheckDuplicateKey(bookingItemDTO.BookingId, bookingItemDTO.ResourceId);
            if (checkDuplicate)
                return CustomResult("Duplicate key violation.");

            var result = await action.Invoke();
            return CustomResult(result);
        }

        private async Task<IActionResult> ProcessActionBulk(IEnumerable<BookingItemDTO> bookingItemsDTOs, Func<Task<IEnumerable<BookingItemDTO>>> action)
        {
            var invalidBookIds = await GetInvalidBookIds(bookingItemsDTOs);
            if (invalidBookIds.Count > 0)
                return CustomResult($"No Client Book found for Ids: {string.Join(", ", invalidBookIds)}");

            var invalidResourceIds = await GetInvalidResourceIds(bookingItemsDTOs);
            if (invalidResourceIds.Count > 0)
                return CustomResult($"No Resource found for Ids: {string.Join(", ", invalidResourceIds)}");

            var duplicateItems = await GetDuplicateItems(bookingItemsDTOs);
            if (duplicateItems.Count > 0)
                return CustomResult($"Duplicate key violation.");

            var result = await action.Invoke();
            return CustomResult(result);
        }

        private async Task<List<int>> GetInvalidBookIds(IEnumerable<BookingItemDTO> bookingItemsDTOs)
        {
            var invalidBookIds = new List<int>();

            foreach (var bookingItemDTO in bookingItemsDTOs)
            {
                bool existenceOfClientBook = await bookingItemRepo.IsClientBookExis(bookingItemDTO.BookingId);
                if (!existenceOfClientBook)
                    invalidBookIds.Add(bookingItemDTO.BookingId);
            }

            return invalidBookIds;
        }

        private async Task<List<int>> GetInvalidResourceIds(IEnumerable<BookingItemDTO> bookingItemsDTOs)
        {
            var invalidResourceIds = new List<int>();

            foreach (var bookingItemDTO in bookingItemsDTOs)
            {
                bool existenceofResource = await bookingItemRepo.IsResourecExist(bookingItemDTO.ResourceId);
                if (!existenceofResource)
                    invalidResourceIds.Add(bookingItemDTO.ResourceId);
            }

            return invalidResourceIds;
        }

        private async Task<List<BookingItemDTO>> GetDuplicateItems(IEnumerable<BookingItemDTO> bookingItemsDTO)
        {
            var duplicateItems = new List<BookingItemDTO>();

            foreach (var bookingItemDTO in bookingItemsDTO)
            {
                bool checkDuplicate = await bookingItemRepo.CheckDuplicateKey(bookingItemDTO.BookingId, bookingItemDTO.ResourceId);
                if (checkDuplicate)
                    duplicateItems.Add(bookingItemDTO);
            }

            return duplicateItems;
        }

    }
}
