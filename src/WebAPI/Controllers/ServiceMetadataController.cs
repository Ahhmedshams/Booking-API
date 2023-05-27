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
            var metadata = await metadataRepo.GetAllAsync();
            if (metadata.Count() == 0)
                return CustomResult("No Service Metadata Found" , HttpStatusCode.NotFound);

            var metadataDTO = mapper.Map<IEnumerable<ServiceMetadata>, IEnumerable<ServiceMetadataDTO>>(metadata);
            return CustomResult(metadataDTO);
        }

        [HttpGet("{serviceId:int}/{resTypeId:int}")]
        public async Task<IActionResult> GetById(int serviceId, int resTypeId)
        {
            var metadata = await metadataRepo.GetServiceMDByIdAsync(serviceId, resTypeId);


            if (metadata == null)
                return CustomResult($"No Service Metadata Found For This service Id: {serviceId} with resource Id: {resTypeId})",
                    HttpStatusCode.NotFound);

            var metadataDTO = mapper.Map<ServiceMetadata, ServiceMetadataDTO>(metadata);
            return CustomResult(metadataDTO);
        }


        [HttpPost]
        public async Task<IActionResult> Add(ServiceMetadataDTO serviceMetadataDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessAction(serviceMetadataDTO, async () =>
            {
                var metadata = mapper.Map<ServiceMetadataDTO, ServiceMetadata>(serviceMetadataDTO);
                await metadataRepo.AddAsync(metadata);
                return serviceMetadataDTO;
            });
        }

        [HttpPut("{serviceId:int}/{resTypeId:int}")]
        public async Task<IActionResult> Edit(int serviceId, int resTypeId, ServiceMetadataDTO serviceMetadata)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessAction(serviceMetadata, async () =>
            {
                var service = mapper.Map<ServiceMetadataDTO, ServiceMetadata>(serviceMetadata);
                await metadataRepo.EditServiceMDAsyn(serviceId, resTypeId, service);
                return serviceMetadata;
            });
        }

        private async Task<IActionResult> ProcessAction(ServiceMetadataDTO serviceMetadata, Func<Task<ServiceMetadataDTO>> action)
        {
            int existenceofClientBookAndRes = await metadataRepo
                                               .CheckExistenceOfServiceIdAndResId(serviceMetadata.ServiceId, 
                                                                                  serviceMetadata.ResourceTypeId);
            switch (existenceofClientBookAndRes)
            {
                case 1:
                    return CustomResult($"No Service found for Id: {serviceMetadata.ServiceId}");
                case -1:
                    return CustomResult($"No Resource Type found for Id: {serviceMetadata.ResourceTypeId}");
            }

            bool checkDuplicate = await metadataRepo.CheckDuplicateKey(serviceMetadata.ServiceId, serviceMetadata.ResourceTypeId);
            if (checkDuplicate)
                return CustomResult("Duplicate key violation. The specified key already exists in the system");

            var result = await action.Invoke();
            return CustomResult(result);
        }














        //[HttpPost]
        //public async Task<IActionResult> Add(ServiceMetadataDTO metadataDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return CustomResult(ModelState, HttpStatusCode.BadRequest);
        //    var metadata = mapper.Map<ServiceMetadataDTO, ServiceMetadata>(metadataDTO);
        //    await metadataRepo.AddAsync(metadata);
        //    return CustomResult(metadataDTO);
        //}

        //[HttpPut("{serviceId:int}/{resourceId:int}")]
        //public async Task<IActionResult> Edit(int serviceId, int resourceId, ServiceMetadataDTO metadataDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return CustomResult(ModelState, HttpStatusCode.BadRequest);

        //    var metadata = mapper.Map<ServiceMetadataDTO, ServiceMetadata>(metadataDTO);

        //    var existingMetadata = await metadataRepo.EditServiceMDAsyn(serviceId, resourceId, metadata);
        //    if (existingMetadata == null)
        //        return CustomResult(HttpStatusCode.NotFound);

        //    metadata.Service = existingMetadata.Service;
        //    metadata.ResourceType = existingMetadata.ResourceType;

        //    return CustomResult(metadataDTO);
        //}

        [HttpDelete("{serviceId:int}/{resTypeId:int}")]
        public async Task<IActionResult> Delete(int serviceId, int resTypeId)
        {
            var bookingItem = await GetById(serviceId, resTypeId);
            if (bookingItem == null)
                return CustomResult(HttpStatusCode.NotFound);

            await metadataRepo.DeleteServiceMDAsyn(serviceId, resTypeId);
            return CustomResult(HttpStatusCode.NoContent);
        }

    }
}
