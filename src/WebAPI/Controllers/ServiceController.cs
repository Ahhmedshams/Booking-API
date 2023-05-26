
using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IAsyncRepository<Service> serviceRepo;
        private readonly IMapper mapper;

        public ServiceController(IAsyncRepository<Service> _serviceRepo,
                                IMapper _mapper)
        {
            serviceRepo = _serviceRepo;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await serviceRepo.GetAllAsync();
            if (services.Count() == 0)
                return BadRequest("No Services Found");

            var servicesDTO = mapper.Map<IEnumerable<Service>, IEnumerable<ServiceDTO>>(services);
            return Ok(servicesDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await serviceRepo.GetByIdAsync(id);
            if (service == null)
                return BadRequest($"No service found for this Id {id}");

            var serviceDTO = mapper.Map<Service , ServiceDTO>(service); 
            return Ok(serviceDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServiceDTO serviceDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var service = mapper.Map<ServiceDTO, Service>(serviceDTO);
            await serviceRepo.AddAsync(service);
            return Ok(service);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id ,ServiceDTO serviceDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var service = mapper.Map<ServiceDTO, Service>(serviceDTO);
            await serviceRepo.EditAsync(id, service , s=>s.Id);
            return Ok(service);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = GetById(id);
            if (service == null)
                return BadRequest($"No service found for this Id {id} to delete");
            await serviceRepo.DeleteAsync(id);
            return NoContent();
        }

    }
}
