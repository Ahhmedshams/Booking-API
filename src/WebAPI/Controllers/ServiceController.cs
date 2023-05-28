
using AutoMapper;
using CoreApiResponse;
using Infrastructure.Persistence.Specification;
using Infrastructure.Persistence.Specification.ServiceSpec;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : BaseController
    {
        private readonly IServiceRepo serviceRepo;
        private readonly IMapper mapper;

        public ServiceController(IServiceRepo _serviceRepo,
                                IMapper _mapper)
        {
            serviceRepo = _serviceRepo;
            mapper = _mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ServiceSpecParams specParams)
        {
            var spec = new ServiceSpecification(specParams);
            var services = await serviceRepo.GetAllServicesWithSpec(spec);
            if (services.Count() == 0)
                return CustomResult("No Services Found", HttpStatusCode.NotFound);

            var servicesDTO = mapper.Map<IEnumerable<Service>, IEnumerable<ServiceDTO>>(services);
            return CustomResult(servicesDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServiceDTO serviceDTO)
        {
            serviceDTO.Id = 0;
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            if (!Enum.IsDefined(typeof(ServiceStatus), serviceDTO.Status))
                return CustomResult("Invalid value for ServiceStatus");

            var service = mapper.Map<ServiceDTO, Service>(serviceDTO);
            await serviceRepo.AddAsync(service);
            serviceDTO.Id = service.Id;
            return CustomResult(serviceDTO);
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
            var service = await serviceRepo.GetServiceById(id);
            if (service == null)
                return CustomResult($"No Service Found For This Id {id}", HttpStatusCode.NotFound);
            await serviceRepo.DeleteSoft(id);
            return CustomResult(HttpStatusCode.NoContent);
        }

    }
}
