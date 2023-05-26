using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientBookingController : ControllerBase
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
                return BadRequest("No Clint's Book found");
            var clientBooksDTO = mapper.Map<IEnumerable<ClientBooking>,IEnumerable<ClientBookingDTO>>(clientBooks);
            return Ok(clientBooksDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var clientBook = await clientBookingRepo.GetByIdAsync(id);
            if (clientBook == null)
                return BadRequest($"No Clint's Book found for this Id {id}");

            var clientBookingDTO = mapper.Map<ClientBooking, ClientBookingDTO>(clientBook);
            return Ok(clientBookingDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientBookingReqDTO clientBookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var clientBook = mapper.Map<ClientBookingReqDTO, ClientBooking>(clientBookDTO);
            await clientBookingRepo.AddAsync(clientBook);
            return Ok(clientBook);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, ClientBookingReqDTO clientBookDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var clientBook = mapper.Map<ClientBookingReqDTO, ClientBooking>(clientBookDTO);
            await clientBookingRepo.EditAsync(id, clientBook, c => c.Id);
            return Ok(clientBook);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = GetById(id);
            if (service == null)
                return BadRequest($"No Client's Book found for this Id {id} to delete it.");
            await clientBookingRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
