using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
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
        private readonly IServiceRepo serviceRepo;
        private readonly IMapper mapper;

        public ServiceMetadataController(IServiceMetadataRepo _metadataRepo,
                                         IServiceRepo _serviceRepo,
                                        IMapper _mapper)
        {
            metadataRepo = _metadataRepo;
            serviceRepo = _serviceRepo;
            mapper = _mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ServiceMetadataSpecParams specParams)
        {
            var spec = new ServiceMetadataSpecification(specParams);
            var metadata = await metadataRepo.GetAllServiceMDWithSpec(spec);
            
            var metadataDTO = mapper.Map<IEnumerable<ServiceMetadata>, 
                                         IEnumerable<ServiceMetadataDTO>>(metadata);
            return CustomResult(metadataDTO);
        }

        [HttpPost("AddOne")]
        public async Task<IActionResult> AddOne(ServiceMetadataDTO serviceMetadataDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            return await ProcessAddOne(serviceMetadataDTO, async () =>
            {
                var metadata = mapper.Map<ServiceMetadataDTO, ServiceMetadata>(serviceMetadataDTO);
                await metadataRepo.AddAsync(metadata);
                return serviceMetadataDTO;
            });
        }

       

        [HttpDelete("DeleteOne")]
        public async Task<IActionResult> Delete([FromQuery] int serviceId, [FromQuery] int resTypeId)
        {
            var servicesMetadata = await metadataRepo.GetById(serviceId, resTypeId);
            if (servicesMetadata == null)
                return CustomResult(HttpStatusCode.NotFound);

            await metadataRepo.DeleteOne(serviceId, resTypeId);
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


        
        private async Task<IActionResult> ProcessAddOne(ServiceMetadataDTO serviceMetadataDTO, 
                                                           Func<Task<ServiceMetadataDTO>> action)
        {
            bool existenceOfServiceId = await metadataRepo.IsServiceExis(serviceMetadataDTO.ServiceId);
            if (!existenceOfServiceId)
                return CustomResult($"No Service found for Id: {serviceMetadataDTO.ServiceId}", HttpStatusCode.BadRequest);

            bool existenceofResTypeId = await metadataRepo.IsResTypeExist(serviceMetadataDTO.ResourceTypeId);
            if (!existenceofResTypeId)
                return CustomResult($"No Resource Type found for Id: {serviceMetadataDTO.ResourceTypeId}", HttpStatusCode.BadRequest);


            bool checkDuplicate = await metadataRepo.CheckDuplicateKey(serviceMetadataDTO.ServiceId, serviceMetadataDTO.ResourceTypeId);
            if (checkDuplicate)
                return CustomResult("Duplicate key violation.", HttpStatusCode.BadRequest);

            var result = await action.Invoke();
            return CustomResult(result);
        }




        [HttpPost("AddBulk")]
        public async Task<IActionResult> AddRange(int serviceId, ServiceMDReqDTO[] serviceMetadataDTOs)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            return await ProcessAddBulk(serviceId,serviceMetadataDTOs, async () =>
            {
                var serviceMDs = await ConvertDtoToServiceMD(serviceId, serviceMetadataDTOs);
                await metadataRepo.AddBulk(serviceMDs);
                return serviceMDs;

            });
        }

        
        private async Task<IActionResult> ProcessAddBulk(int serviceId,ServiceMDReqDTO[] serviceMetadataDTOs,
                                                            Func<Task<dynamic>> action)
        {
            var invalidServiceIds = await metadataRepo.IsServiceExis(serviceId);
            if (!invalidServiceIds)
                return CustomResult($"No Service found for Id: {serviceId}", HttpStatusCode.BadRequest);

            var invalidResTypeIds = await GetInvalidResourceTypeIds(serviceMetadataDTOs);
            if (invalidResTypeIds.Count > 0)
                return CustomResult($"No Resource Type found for Ids: {string.Join(", ", invalidResTypeIds)}", HttpStatusCode.BadRequest);

            var duplicateItems = await GetDuplicateItems(serviceId,serviceMetadataDTOs);
            if (duplicateItems.Count > 0)
                return CustomResult($"Duplicate key violation", HttpStatusCode.BadRequest);

            var result = await action.Invoke();
            var ResourceTypes = serviceMetadataDTOs.Select(s => s.ResourceTypeId);
            return CustomResult(new {ServiceId = serviceId, ResourceTypes });
        }
        private async Task<List<ServiceMDReqDTO>> GetInvalidResourceTypeIds(ServiceMDReqDTO[] serviceMetadataDTOs)
        {
            var invalidResTypeIds = new List<ServiceMDReqDTO>();

            foreach (var serviceMDReqDTO in serviceMetadataDTOs)
            {
                bool existenceofResTypeId = await metadataRepo.IsResTypeExist(serviceMDReqDTO.ResourceTypeId);
                if (!existenceofResTypeId)
                    invalidResTypeIds.Add(serviceMDReqDTO);
            }

            return invalidResTypeIds;
        }

        
        private async Task<List<ServiceMDReqDTO>> GetDuplicateItems(int serviceId,ServiceMDReqDTO[] serviceMetadataDTOs)
        {
            var duplicateItems = new List<ServiceMDReqDTO>();

            foreach (var serviceMDReqDTO in serviceMetadataDTOs)
            {
                bool checkDuplicate = await metadataRepo.CheckDuplicateKey(serviceId,serviceMDReqDTO.ResourceTypeId);
                if (checkDuplicate)
                    duplicateItems.Add(serviceMDReqDTO);
            }

            return duplicateItems;
        }

		private async Task<IEnumerable<ServiceMetadata>> ConvertDtoToServiceMD(int serviceId, ServiceMDReqDTO[] serviceMDReqDTO)
		{
			List<ServiceMetadata> serviceMetadata = new();

			foreach (var dto in serviceMDReqDTO)
			{
				ServiceMetadata serviceMD = new ServiceMetadata(); // Create a new instance for each object
				serviceMD.ServiceId = serviceId;
				serviceMD.ResourceTypeId = dto.ResourceTypeId;
				serviceMetadata.Add(serviceMD);
			}

			return serviceMetadata;
		}



		//[HttpPut("EditOne")]
		//public async Task<IActionResult> Edit([FromQuery] int serviceId,
		//                                      ServiceMetadataUpdateDTO serviceMetadata)
		//{
		//    if (!ModelState.IsValid)
		//        return CustomResult(ModelState, HttpStatusCode.BadRequest);

		//    return await ProcessUpdateOne(serviceId, serviceMetadata, async () =>
		//    {
		//        await metadataRepo.EditOne(serviceId, serviceMetadata.ResourceTypeId);
		//        return serviceMetadata;
		//    });
		//}



		//[HttpPut("EditBulk")]
		//public async Task<IActionResult> EditBulk(IEnumerable<int> resourceTypes,
		//                                      [FromQuery] int serviceId)
		//{
		//    if (!ModelState.IsValid)
		//        return CustomResult(ModelState, HttpStatusCode.BadRequest);
		//    return await ProcessEditBulk(serviceId, resourceTypes, async () =>
		//    {
		//        await metadataRepo.EditBulk(serviceId, resourceTypes);
		//        return resourceTypes;

		//    });

		//}

		//private async Task<IActionResult> ProcessUpdateOne(int serviceId,ServiceMetadataUpdateDTO serviceMetadataDTO,
		//                                                  Func<Task<ServiceMetadataUpdateDTO>> action)
		//{
		//    var service = await serviceRepo.GetByIdAsync(serviceId);
		//    if (service == null)
		//        return CustomResult($"No service found for Id: {serviceId}", HttpStatusCode.BadRequest);

		//    bool invalidResTypeId = await metadataRepo.IsResTypeExist(serviceMetadataDTO.ResourceTypeId);
		//    if (!invalidResTypeId)
		//        return CustomResult($"No Resource Type found for Id: {serviceMetadataDTO.ResourceTypeId}", HttpStatusCode.BadRequest);

		//    var result = await action.Invoke();
		//    return CustomResult(new {ServiceId = serviceId, ResourceTypeId = serviceMetadataDTO.ResourceTypeId});
		//}


		//private async Task<IActionResult> ProcessEditBulk(int serviceId,IEnumerable<int> resourceTypes,
		//                                                  Func<Task<IEnumerable<int>>> action)
		//{
		//    var service = await serviceRepo.GetByIdAsync(serviceId);
		//    if (service == null)
		//        return CustomResult($"No service found for Id: {serviceId}", HttpStatusCode.BadRequest);

		//    var invalidResTypeIds = await GetInvalidResourceTypeIds2(resourceTypes);
		//    if (invalidResTypeIds.Count > 0)
		//        return CustomResult($"No Resource Type found for Ids: {string.Join(", ", invalidResTypeIds)}", HttpStatusCode.BadRequest);

		//    var result = await action.Invoke();
		//    return CustomResult(new {ServiceId = serviceId, ResoureTypes = resourceTypes});
		//}

	}
}
