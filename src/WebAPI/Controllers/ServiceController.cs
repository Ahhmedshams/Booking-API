
using AutoMapper;
using CoreApiResponse;
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
        private readonly UploadImage _uploadImage;
        public ServiceController(IServiceRepo _serviceRepo,IMapper _mapper, UploadImage uploadImage)
        {
            serviceRepo = _serviceRepo;
            mapper = _mapper;
            _uploadImage = uploadImage;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ServiceSpecParams specParams)
        {
            var spec = new ServiceSpecification(specParams);
            var services = await serviceRepo.GetAllServicesWithSpec(spec);
            var servicesDTO = mapper.Map<IEnumerable<Service>, IEnumerable<ServiceDTO>>(services);
            return CustomResult(servicesDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add( [FromForm] ServiceDTO serviceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);


            if (!Enum.IsDefined(typeof(ServiceStatus), serviceDTO.Status))
                return CustomResult("Invalid value for ServiceStatus", HttpStatusCode.BadRequest);

            var service = mapper.Map<ServiceDTO, Service>(serviceDTO);

            if (serviceDTO.UploadedImage != null)
                service.Image = await _uploadImage.UploadToCloud(serviceDTO.UploadedImage);

            await serviceRepo.AddAsync(service);
            return CustomResult(serviceDTO);
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromQuery] int id ,ServiceDTO serviceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            if (!Enum.IsDefined(typeof(ServiceStatus), serviceDTO.Status))
                return CustomResult("Invalid value for ServiceStatus", HttpStatusCode.BadRequest);

            var service = mapper.Map<ServiceDTO, Service>(serviceDTO);
            await serviceRepo.EditAsync(id, service , s=>s.Id);
            return CustomResult(service);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var service = await serviceRepo.GetServiceById(id);
            if (service == null)
                return CustomResult($"No Service Found For This Id {id}", HttpStatusCode.NotFound);
            await serviceRepo.DeleteSoft(id);
            return CustomResult(HttpStatusCode.NoContent);
        }

    }
}
