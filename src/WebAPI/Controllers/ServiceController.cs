
using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : BaseController
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
                return CustomResult("No Services Found", HttpStatusCode.NotFound);

            var servicesDTO = mapper.Map<IEnumerable<Service>, IEnumerable<ServiceDTO>>(services);
            return CustomResult(servicesDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await serviceRepo.GetByIdAsync(id);
            if (service == null)
                return CustomResult($"No Service Found For This Id {id}", HttpStatusCode.NotFound);

            var serviceDTO = mapper.Map<Service , ServiceDTO>(service); 
            return CustomResult(serviceDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServiceDTO serviceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            if (!Enum.IsDefined(typeof(ServiceStatus), serviceDTO.Status))
                return CustomResult("Invalid value for ServiceStatus");

            var service = mapper.Map<ServiceDTO, Service>(serviceDTO);
            await serviceRepo.AddAsync(service);
            return CustomResult(service);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id ,ServiceDTO serviceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            if (!Enum.IsDefined(typeof(ServiceStatus), serviceDTO.Status))
                return CustomResult("Invalid value for ServiceStatus");

            var service = mapper.Map<ServiceDTO, Service>(serviceDTO);
            await serviceRepo.EditAsync(id, service , s=>s.Id);
            return CustomResult(service);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await GetById(id);
            if (service == null)
                return CustomResult($"No Service Found For This Id {id}", HttpStatusCode.NotFound);
            await serviceRepo.DeleteAsync(id);
            return CustomResult(HttpStatusCode.NoContent);
        }

    }
}
