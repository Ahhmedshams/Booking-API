using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceMetadataController : ControllerBase
    {
        private readonly IServiceMetadataRepo metadataRepo;
        private readonly IMapper mapper;

        public ServiceMetadataController(IServiceMetadataRepo _metadataRepo,
                                        IMapper _mapper)
        {
            metadataRepo = _metadataRepo;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var metadata = await metadataRepo.GetAllAsync(true , s=> s.Service , r => r.ResourceType);
            if (metadata.Count() == 0)
                return BadRequest("No Service Metadat Found");

            var metadataDTO = mapper.Map<IEnumerable<ServiceMetadata>, IEnumerable<ServiceMetadataResDTO>>(metadata);
            return Ok(metadataDTO);
        }

        [HttpGet("{serviceId:int}/{resourceId:int}")]
        public async Task<IActionResult> GetById(int serviceId, int resourceId)
        {
            var metadata = await metadataRepo.GetServiceMDByIdAsync(serviceId, resourceId,
                s => s.Service, r => r.ResourceType);


            if (metadata == null)
                return BadRequest($"No service metadata found for this  ({serviceId}, {resourceId})");

            var metadataDTO = mapper.Map<ServiceMetadata, ServiceMetadataResDTO>(metadata);
            return Ok(metadataDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServiceMetadataReqDTO metadataDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var metadata = mapper.Map<ServiceMetadataReqDTO, ServiceMetadata>(metadataDTO);
            await metadataRepo.AddAsync(metadata);
            return Ok(metadata);
        }

        [HttpPut("{serviceId:int}/{resourceId:int}")]
        public async Task<IActionResult> Edit(int serviceId, int resourceId, ServiceMetadataReqDTO metadataDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var metadata = mapper.Map<ServiceMetadataReqDTO, ServiceMetadata>(metadataDTO);
            await metadataRepo.EditServiceMDAsyn(serviceId, resourceId, metadata);
            return Ok(metadata);
        }

        [HttpDelete("{serviceId:int}/{resourceId:int}")]
        public async Task<IActionResult> Delete(int serviceId, int resourceId)
        {
            var serviceMD = GetById(serviceId, resourceId);
            if (serviceMD == null)
                return BadRequest("No Booking Item found to delete it");
            await metadataRepo.DeleteServiceMDAsyn(serviceId, resourceId);
            return NoContent();
        }

    }
}
