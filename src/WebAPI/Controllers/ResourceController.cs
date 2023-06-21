using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IResourceRepo _resourceRepo;
        private readonly IResourceTypeRepo _resourceTypeRepo;
        private readonly IResourceDataRepo _resourceDataRepo;
        private readonly IResourceMetadataRepo _resourceMetadataRepo;

        public ResourceController(IMapper mapper, IResourceRepo resourceRepo, IResourceTypeRepo resourceTypeRepo, IResourceDataRepo resourceDataRepo, IResourceMetadataRepo resourceMetadataRepo)
        {
            _mapper = mapper;
            _resourceRepo = resourceRepo;
            _resourceTypeRepo = resourceTypeRepo;
            _resourceDataRepo = resourceDataRepo;
            _resourceMetadataRepo = resourceMetadataRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Resource> resource = await _resourceRepo.GetAllAsync(true , r=> r.Region);
            if (resource.Count() == 0)
                return CustomResult("No Resource Are Available", HttpStatusCode.NotFound);

            List<ResourceRespDTO> resourceDTO = _mapper.Map<List<ResourceRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpGet("ResourceType/{id:int}")]
        public async Task<IActionResult> GetAllByResourceType(int id)
        {
            IEnumerable<Resource> resource =  _resourceRepo.Find(e=>e.ResourceTypeId==id);
            if (resource.Count() == 0)
                return CustomResult("No Resource Are Available", HttpStatusCode.NotFound);

            List<ResourceRespDTO> resourceDTO = _mapper.Map<List<ResourceRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            Resource resource = await _resourceRepo.GetByIdAsync(id);
            if (resource == null)
                return CustomResult($"No Resource Type Are Available With id {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<ResourceRespDTO>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ResourceReqDTO resourceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var ResoureceType = await _resourceTypeRepo.IsExistAsync(resourceDTO.ResourceTypeId);
            if (!ResoureceType)
                return CustomResult($"No Resource Type Are Available With id {resourceDTO.ResourceTypeId}", HttpStatusCode.BadRequest);

           
            var resource = _mapper.Map<Resource>(resourceDTO);
            var result = await _resourceRepo.AddAsync(resource);
            return CustomResult(result);
            
        }

        [HttpPost("WithData")]
        public async Task<IActionResult> CreateResourceWithData(ResourceWithDataDTO ResourceWithDataDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var ResoureceType = await _resourceTypeRepo.IsExistAsync(ResourceWithDataDTO.ResourceTypeId);
            if (!ResoureceType)
                return CustomResult($"No Resource Type Are Available With id {ResourceWithDataDTO.ResourceTypeId}", HttpStatusCode.BadRequest);


            var resource = _mapper.Map<Resource>(ResourceWithDataDTO);
            var result = await _resourceRepo.AddAsync(resource);

            (IActionResult ResourceDataValidation, List<ResourceData> resourceData) = await CheckResourceData(result, ResourceWithDataDTO.ResourceAttributes);
            if (ResourceDataValidation != null)
                return ResourceDataValidation;

            var res = await _resourceDataRepo.AddRangeAsync(resourceData);

            var resDTO = _mapper.Map<ResourceDataRespDTO>(res);

            return CustomResult(resDTO);
        }





        [HttpPut("{id:int}")]
        public IActionResult Edit(int id,[FromBody] Decimal price)
        {
            if (price <= 0)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var result = _resourceRepo.EditPrice(id, price);

            return CustomResult(result);
        }



        [HttpDelete("SoftDelete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            if (id == 0)
                return CustomResult($"No Resource Is Available With id {id}", HttpStatusCode.BadRequest);

            await _resourceRepo.SoftDeleteAsync(id);

            return CustomResult(HttpStatusCode.NoContent);
        }



        private async Task<(IActionResult, List<ResourceData>)> CheckResourceData(Resource resource, List<ResourceDataRespIDValueDTO> resourceDTO)
        {

            List<ResourceData> resourceDatas = new List<ResourceData>();
         
            //Get Resource Type MetaData
            var resourceMetaData = await _resourceMetadataRepo.FindAsync(Re => Re.ResourceTypeId == resource.ResourceTypeId);
            if (resourceMetaData.Count() == 0)
                return (CustomResult($"No Resource Metadata Available for ResourceType ID {resource.Id}", HttpStatusCode.NotFound), null);

            foreach (var item in resourceDTO)
            {
                //Check If Attribute Id Exist
                ResourceMetadata Attribute = resourceMetaData.FirstOrDefault(res => res.AttributeId == item.AttributeId);
                if (Attribute == null)
                    return (CustomResult(message: $"No AttributeId Available With ID {item.AttributeId}", HttpStatusCode.NotFound), null);

                //Check If Attribute Id and Resource ID Inserted Before 
                bool IsExist = await _resourceDataRepo.IsExistAsync(res => res.AttributeId == item.AttributeId && res.ResourceId == resource.Id);
                if (IsExist)
                    return (CustomResult($"The Attribute with ID {item.AttributeId} Already inserted before", HttpStatusCode.NotFound), null);


                //Validate Attribute value
                var ValueResult = ValidateValue(Attribute.AttributeType, item.AttributeValue, item.AttributeId);

                if (ValueResult != null)
                    return (ValueResult, null);

                ResourceData resourceData = new()
                {
                    ResourceId = resource.Id,
                    AttributeId = item.AttributeId,
                    AttributeValue = item.AttributeValue
                };

                resourceDatas.Add(resourceData);
            }





            return (null, resourceDatas);

        }

        private IActionResult ValidateValue(string Type, string Value, int AttId)
        {
            switch (Type)
            {
                case "Number":

                    if (Int32.TryParse(Value, out int value))
                        break;
                    else
                        return CustomResult(message: $"Not a Valid Value for Attribute Value ID {AttId}", HttpStatusCode.BadRequest);
                case "Boolean":
                    if (Value.ToLower() == "true" || Value.ToLower() == "false")
                        break;
                    else
                        return CustomResult(message: $"Not a Valid Value for Attribute Value ID {AttId}", HttpStatusCode.BadRequest);
                case "Date":
                    if (DateTime.TryParse(Value, out DateTime DateValue))
                        break;
                    else
                        return CustomResult(message: $"Not a Valid Value for Attribute Value ID {AttId}", HttpStatusCode.BadRequest);
            }

            return null;
        }
    }
}
