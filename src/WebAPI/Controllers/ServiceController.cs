
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Infrastructure.Persistence.Specification.ServiceSpec;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
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

        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;
        public ServiceController(IServiceRepo _serviceRepo,IMapper _mapper, UploadImage uploadImage, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions
)
        {
			serviceRepo = _serviceRepo;
            mapper = _mapper;
            _uploadImage = uploadImage;
            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ServiceSpecParams specParams,[FromQuery] SieveModel sieveModel)
        {
            var spec = new ServiceSpecification(specParams);
            var services = await serviceRepo.GetAllServicesWithSpec(spec);
            var servicesDTO = mapper.Map<IEnumerable<Service>, IEnumerable<ServiceResDTO>>(services);
            var FilteredService = _sieveProcessor.Apply(sieveModel, servicesDTO.AsQueryable());

            return CustomResult(FilteredService);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ServiceDTO serviceDTO)
        {
            try
            {
				if (!ModelState.IsValid)
					return CustomResult(ModelState, HttpStatusCode.BadRequest);


				if (!Enum.IsDefined(typeof(ServiceStatus), serviceDTO.Status))
					return CustomResult("Invalid value for ServiceStatus", HttpStatusCode.BadRequest);

				var service = mapper.Map<ServiceDTO, Service>(serviceDTO);

				if (serviceDTO.UploadedImages != null && serviceDTO.UploadedImages.Any())
				{
					var entityType = "ServiceImage";
					var images = await _uploadImage.UploadToCloud(serviceDTO.UploadedImages, entityType);

					if (images != null && images.Any())
					{
						var serviceImages = images.OfType<ServiceImage>().ToList();
						service.Images = serviceImages;
					}
				}

				await serviceRepo.AddAsync(service);
                ServiceResDTO serviceResDTO = mapper.Map<Service, ServiceResDTO>(service);
				return CustomResult(serviceResDTO);
			}
			catch(Exception ex)
            {
				return CustomResult("Duplicate Service Name", HttpStatusCode.BadRequest);
			}
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

        [HttpGet("region/{RegionId:int}")]
        public async Task<IActionResult> GetServicesByRegion(int RegionId)
        {
            var service = await serviceRepo.ServicesByRegion(RegionId);
            return CustomResult(mapper.Map<List<ServiceRegionDTO>>(service));

        }

    }
}
