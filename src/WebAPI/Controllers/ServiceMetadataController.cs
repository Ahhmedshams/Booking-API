using Application.Common.Interfaces.Repositories;
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
    public class ServiceMetadataController : BaseController
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
                return CustomResult("No Service Metadata Found" , HttpStatusCode.NotFound);

            var metadataDTO = mapper.Map<IEnumerable<ServiceMetadata>, IEnumerable<ServiceMetadataResDTO>>(metadata);
            return CustomResult(metadataDTO);
        }

        [HttpGet("{serviceId:int}/{resourceId:int}")]
        public async Task<IActionResult> GetById(int serviceId, int resourceId)
        {
            var metadata = await metadataRepo.GetServiceMDByIdAsync(serviceId, resourceId,
                s => s.Service, r => r.ResourceType);


            if (metadata == null)
                return CustomResult($"No Service Metadata Found For This service Id: {serviceId} with resource Id: {resourceId})",
                    HttpStatusCode.NotFound);

            var metadataDTO = mapper.Map<ServiceMetadata, ServiceMetadataResDTO>(metadata);
            return CustomResult(metadataDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServiceMetadataReqDTO metadataDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            var metadata = mapper.Map<ServiceMetadataReqDTO, ServiceMetadata>(metadataDTO);
            await metadataRepo.AddAsync(metadata);
            return CustomResult(metadata);
        }

        //[HttpPut("{serviceId:int}/{resourceId:int}")]
        //public async Task<IActionResult> Edit(int serviceId, int resourceId, ServiceMetadataReqDTO metadataDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return CustomResult(ModelState, HttpStatusCode.BadRequest);

        //    var metadata = mapper.Map<ServiceMetadataReqDTO, ServiceMetadata>(metadataDTO);

        //    var existingMetadata = await metadataRepo.EditServiceMDAsyn(serviceId, resourceId, metadata);
        //    if (existingMetadata == null)
        //        return CustomResult(HttpStatusCode.NotFound);

        //    metadata.Service = existingMetadata.Service;
        //    metadata.ResourceType = existingMetadata.ResourceType;

        //    return CustomResult(metadata);
        //}

        [HttpDelete("{serviceId:int}/{resourceId:int}")]
        public async Task<IActionResult> Delete(int serviceId, int resourceId)
        {
            var bookingItem = await GetById(serviceId, resourceId);
            if (bookingItem == null)
                return CustomResult(HttpStatusCode.NotFound);
            await metadataRepo.DeleteServiceMDAsyn(serviceId, resourceId);
            return CustomResult(HttpStatusCode.NoContent);
        }

    }
}
