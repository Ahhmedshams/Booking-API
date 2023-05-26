using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientBookingController : BaseController
    {
        private readonly IAsyncRepository<ClientBooking> clientBookingRepo;
        private readonly IMapper mapper;

        public ClientBookingController(IAsyncRepository<ClientBooking> _clientBookingRepo,
                                        IMapper _mapper)
        {
            clientBookingRepo = _clientBookingRepo;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clientBooks = await clientBookingRepo.GetAllAsync();
            if (clientBooks.Count() == 0)
                return CustomResult("No Clint's Book Found", HttpStatusCode.NotFound);
            var clientBooksDTO = mapper.Map<IEnumerable<ClientBooking>,IEnumerable<ClientBookingDTO>>(clientBooks);
            return CustomResult(clientBooksDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var clientBook = await clientBookingRepo.GetByIdAsync(id);
            if (clientBook == null)
                return CustomResult($"No Clint's Book Found for this Id {id}", HttpStatusCode.NotFound);

            var clientBookingDTO = mapper.Map<ClientBooking, ClientBookingDTO>(clientBook);
            return CustomResult(clientBookingDTO);
        }

        //[HttpPost]
        //public async Task<IActionResult> Add(ClientBookingDTO clientBookDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return CustomResult(ModelState , HttpStatusCode.BadRequest);
        //    var clientBook = mapper.Map<ClientBookingDTO, ClientBooking>(clientBookDTO);
        //    await clientBookingRepo.AddAsync(clientBook);
        //    return CustomResult(clientBook);
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Edit(int id, ClientBookingDTO clientBookDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return CustomResult(ModelState , HttpStatusCode.BadRequest);
        //    var clientBook = mapper.Map<ClientBookingDTO, ClientBooking>(clientBookDTO);
        //    await clientBookingRepo.EditAsync(id, clientBook, c => c.Id);
        //    return CustomResult(clientBook);
        //}

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await GetById(id);
            if (service == null)
                return CustomResult($"No Client's Book found for this Id {id}", HttpStatusCode.NotFound);
            await clientBookingRepo.DeleteAsync(id);
            return CustomResult(HttpStatusCode.NoContent);
        }
    }
}
