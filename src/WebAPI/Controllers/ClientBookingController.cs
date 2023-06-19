using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Infrastructure.Persistence.Specification.ClientBookingSpec;
using Infrastructure.Persistence.Specification;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientBookingController : BaseController
    {
        private readonly IClientBookingRepo clientBookingRepo;
        private readonly IMapper mapper;

        public ClientBookingController(IClientBookingRepo _clientBookingRepo,
                                        IMapper _mapper)
        {
            clientBookingRepo = _clientBookingRepo;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ClientBookingSpecParam specParams)
        {
            var spec = new ClientBookingSpecification(specParams);
            var clientBooks = await clientBookingRepo.GetAllBookingsWithSpec(spec);
            
            var clientBooksDTO = mapper.Map<IEnumerable<ClientBooking>, IEnumerable<ClientBookingDTO>>(clientBooks);
            return CustomResult(clientBooksDTO);
        }

        [HttpPost] 
        public async Task<IActionResult> Add(ClientBookingDTO clientBookDTO)
        {
            clientBookDTO.Id = 0;
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            if (!Enum.IsDefined(typeof(BookingStatus), clientBookDTO.Status))
                return CustomResult("Invalid value for BookingStatus", HttpStatusCode.BadRequest);

            //var isServiceExist = await clientBookingRepo.IsServiceExist(clientBookDTO.ServiceId);
            //if (!isServiceExist)
            //    return CustomResult("Service Id is not exist", HttpStatusCode.BadRequest);

            //var isUserExist = await clientBookingRepo.IsUserExist(clientBookDTO.UserId);
            //if (!isUserExist)
            //    return CustomResult("User Id is not exist", HttpStatusCode.BadRequest);

            var clientBook = mapper.Map<ClientBookingDTO, ClientBooking>(clientBookDTO);
            await clientBookingRepo.AddAsync(clientBook);

            clientBookDTO.Id = clientBook.Id;
            return CustomResult(clientBookDTO);
        }



        [HttpPut]
        public async Task<IActionResult> Edit([FromQuery] int id, ClientBookingDTO clientBookDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            //var serviceExisting = await clientBookingRepo.IsServiceExist(clientBookDTO.ServiceId);
            //if (!serviceExisting)
            //    return CustomResult("Service Id is not exist", HttpStatusCode.BadRequest);

            var clientBook = mapper.Map<ClientBookingDTO, ClientBooking>(clientBookDTO);
            await clientBookingRepo.EditAsync(id, clientBook, c => c.Id);
            return CustomResult(clientBookDTO);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var service = await clientBookingRepo.GetBookingById(id);
            if (service == null)
                return CustomResult($"No Client's Book found for this Id {id}", HttpStatusCode.NotFound);
            await clientBookingRepo.DeleteAsync(id);
            return CustomResult(HttpStatusCode.NoContent);
        }
        [HttpPost("CreateNewBooking")]
        public async Task<IActionResult> CreateNewBooking (ClientBooking2DTO clientBooking2DTO)
        {
            int result = await clientBookingRepo.CreateNewBooking
                (clientBooking2DTO.UserID,
                clientBooking2DTO.Date,
                clientBooking2DTO.ServiceID,
                clientBooking2DTO.Location,
                clientBooking2DTO.StartTime,
                clientBooking2DTO.EndTime,
                clientBooking2DTO.ResourceIDs);
            
            if (result==-1)
            {
                return BadRequest("Invalid data entered");
            }
            return CustomResult(result,HttpStatusCode.Created);

        }
    }
}
