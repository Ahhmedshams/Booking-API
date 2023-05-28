using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Infrastructure.Persistence.Specification.ServiceMetadataSpec;
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
        public async Task<IActionResult> GetAll([FromQuery] ServiceMetadataSpecParams specParams)
        {
            var spec = new ServiceMetadataSpecification(specParams);
            var metadata = await metadataRepo.GetAllServiceMDWithSpec(spec);
            if (metadata.Count() == 0)
                return CustomResult("No Service Metadata Found", HttpStatusCode.NotFound);

            var metadataDTO = mapper.Map<IEnumerable<ServiceMetadata>, IEnumerable<ServiceMetadataDTO>>(metadata);
            return CustomResult(metadataDTO);
        }

        [HttpPost("AddOne")]
        public async Task<IActionResult> AddOne(ServiceMetadataDTO serviceMetadataDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessActionOne(serviceMetadataDTO, async () =>
            {
                var metadata = mapper.Map<ServiceMetadataDTO, ServiceMetadata>(serviceMetadataDTO);
                await metadataRepo.AddAsync(metadata);
                return serviceMetadataDTO;
            });
        }

        [HttpPost("AddBulk")]
        public async Task<IActionResult> AddRange(IEnumerable<ServiceMetadataDTO> serviceMetadataDTOs)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessActionBulk(serviceMetadataDTOs, async () =>
            {
                var addedItems = new List<ServiceMetadataDTO>();

                var services = mapper.Map<IEnumerable<ServiceMetadataDTO>, IEnumerable<ServiceMetadata>>(serviceMetadataDTOs);
                await metadataRepo.AddBulk(services);
                return serviceMetadataDTOs;

            });
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromQuery] int serviceId, [FromQuery] int resTypeId, ServiceMetadataDTO serviceMetadata)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await ProcessActionOne(serviceMetadata, async () =>
            {
                var service = mapper.Map<ServiceMetadataDTO, ServiceMetadata>(serviceMetadata);
                await metadataRepo.EditServiceMDAsyn(serviceId, resTypeId, service);
                return serviceMetadata;
            });
        }



        [HttpDelete("DeleteOne")]
        public async Task<IActionResult> Delete([FromQuery] int serviceId, [FromQuery] int resTypeId)
        {
            var servicesMetadata = await metadataRepo.GetServiceMDByIdAsync(serviceId, resTypeId);
            if (servicesMetadata == null)
                return CustomResult(HttpStatusCode.NotFound);

            await metadataRepo.DeleteServiceMDAsyn(serviceId, resTypeId);
            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpDelete("DeleteBulk")]
        public async Task<IActionResult> DeleteBulk([FromQuery] int serviceId)
        {
            var servicesMetadata = await metadataRepo.GetByServiceId(serviceId);
            if (servicesMetadata == null)
                return CustomResult(HttpStatusCode.NotFound);

            await metadataRepo.DeleteBulk(serviceId);
            return CustomResult(HttpStatusCode.NoContent);
        }

        private async Task<IActionResult> ProcessActionOne(ServiceMetadataDTO serviceMetadataDTO, Func<Task<ServiceMetadataDTO>> action)
        {

            bool existenceOfServiceId = await metadataRepo.IsServiceExis(serviceMetadataDTO.ServiceId);
            if (!existenceOfServiceId)
                return CustomResult($"No Service found for Id: {serviceMetadataDTO.ServiceId}");

            bool existenceofResTypeId = await metadataRepo.IsResTypeExist(serviceMetadataDTO.ResourceTypeId);
            if (!existenceofResTypeId)
                return CustomResult($"No Resource Type found for Id: {serviceMetadataDTO.ResourceTypeId}");


            bool checkDuplicate = await metadataRepo.CheckDuplicateKey(serviceMetadataDTO.ServiceId, serviceMetadataDTO.ResourceTypeId);
            if (checkDuplicate)
                return CustomResult("Duplicate key violation. The specified key already exists in the system");

            var result = await action.Invoke();
            return CustomResult(result);
        }

        private async Task<IActionResult> ProcessActionBulk(IEnumerable<ServiceMetadataDTO> serviceMetadataDTOs, Func<Task<IEnumerable<ServiceMetadataDTO>>> action)
        {
            var invalidServiceIds = await GetInvalidServiceIds(serviceMetadataDTOs);
            if (invalidServiceIds.Count > 0)
                return CustomResult($"No Service found for Ids: {string.Join(", ", invalidServiceIds)}");

            var invalidResTypeIds = await GetInvalidResourceTypeIds(serviceMetadataDTOs);
            if (invalidResTypeIds.Count > 0)
                return CustomResult($"No Resource Type found for Ids: {string.Join(", ", invalidResTypeIds)}");

            var duplicateItems = await GetDuplicateItems(serviceMetadataDTOs);
            if (duplicateItems.Count > 0)
                return CustomResult($"Duplicate key violation");

            var result = await action.Invoke();
            return CustomResult(result);
        }

        private async Task<List<int>> GetInvalidServiceIds(IEnumerable<ServiceMetadataDTO> serviceMetadataDTOs)
        {
            var invalidServiceIds = new List<int>();

            foreach (var serviceMetadataDTO in serviceMetadataDTOs)
            {
                bool existenceOfServiceId = await metadataRepo.IsServiceExis(serviceMetadataDTO.ServiceId);
                if (!existenceOfServiceId)
                    invalidServiceIds.Add(serviceMetadataDTO.ServiceId);
            }

            return invalidServiceIds;
        }

        private async Task<List<int>> GetInvalidResourceTypeIds(IEnumerable<ServiceMetadataDTO> serviceMetadataDTOs)
        {
            var invalidResTypeIds = new List<int>();

            foreach (var serviceMetadataDTO in serviceMetadataDTOs)
            {
                bool existenceofResTypeId = await metadataRepo.IsResTypeExist(serviceMetadataDTO.ResourceTypeId);
                if (!existenceofResTypeId)
                    invalidResTypeIds.Add(serviceMetadataDTO.ResourceTypeId);
            }

            return invalidResTypeIds;
        }

        private async Task<List<ServiceMetadataDTO>> GetDuplicateItems(IEnumerable<ServiceMetadataDTO> serviceMetadataDTOs)
        {
            var duplicateItems = new List<ServiceMetadataDTO>();

            foreach (var serviceMetadataDTO in serviceMetadataDTOs)
            {
                bool checkDuplicate = await metadataRepo.CheckDuplicateKey(serviceMetadataDTO.ServiceId, serviceMetadataDTO.ResourceTypeId);
                if (checkDuplicate)
                    duplicateItems.Add(serviceMetadataDTO);
            }

            return duplicateItems;
        }



    }
}
