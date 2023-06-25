using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using WebAPI.DTO;

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
        private readonly UploadImage _uploadImage;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;
        private readonly IRegionRepository _regionRepo;


        public ResourceController(IMapper mapper, IResourceRepo resourceRepo, IResourceTypeRepo resourceTypeRepo, IResourceDataRepo resourceDataRepo, IResourceMetadataRepo resourceMetadataRepo, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions, UploadImage uploadImage, IRegionRepository regionRepo)
        {
            _mapper = mapper;
            _resourceRepo = resourceRepo;
            _resourceTypeRepo = resourceTypeRepo;
            _resourceDataRepo = resourceDataRepo;
            _resourceMetadataRepo = resourceMetadataRepo;

            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value; // Access the value of SieveOptions from IOptions<T>

            _uploadImage = uploadImage;
            _regionRepo = regionRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SieveModel sieveModel)
        {
            IEnumerable<Resource> resource = await _resourceRepo.GetAllAsync(true,e=>e.Region,e=>e.Images);
            if (resource.Count() == 0)
                return CustomResult("No Resource Are Available", HttpStatusCode.NotFound);

            List<ResourceRespDTO> resourceDTO = _mapper.Map<List<ResourceRespDTO>>(resource);
            IQueryable<ResourceRespDTO>? FilteredSchedules = _sieveProcessor.Apply<ResourceRespDTO>(sieveModel, resourceDTO.AsQueryable());

            return CustomResult(FilteredSchedules);
        }

        [HttpGet("ResourceType/{id:int}")]
        public async Task<IActionResult> GetAllByResourceType(int id, [FromQuery] SieveModel sieveModel)
        {

            // Include Region
            IEnumerable<Resource> resource =  _resourceRepo.Find(e=>e.ResourceTypeId==id);
            if (resource.Count() == 0)
                return CustomResult("No Resource Are Available", HttpStatusCode.NotFound);

            List<ResourceRespDTO> resourceDTO = _mapper.Map<List<ResourceRespDTO>>(resource);
            var FilteredSchedules = _sieveProcessor.Apply(sieveModel, resourceDTO.AsQueryable());
            return CustomResult(FilteredSchedules);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            Resource resource = await _resourceRepo.GetByIdAsync(id,e=>e.Region,e=>e.Images);
            if (resource == null)
                return CustomResult($"No Resource Type Are Available With id {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<ResourceRespDTO>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm]ResourceReqDTO resourceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            if (resourceDTO.RegionId!=null)
            {
                var RegionExists = await CheckRegionExists(resourceDTO.RegionId ?? default(int));
                if (!RegionExists) return CustomResult("Region Doesn't Exists",HttpStatusCode.NotFound);
            }

            var ResoureceType = await _resourceTypeRepo.IsExistAsync(resourceDTO.ResourceTypeId);
            if (!ResoureceType)
                return CustomResult($"No Resource Type Are Available With id {resourceDTO.ResourceTypeId}", HttpStatusCode.BadRequest);
           
            var resource = _mapper.Map<Resource>(resourceDTO);

            if (resourceDTO.UploadedImages != null && resourceDTO.UploadedImages.Any())
            {
                var entityType = "ResourceImage";
                var images = await _uploadImage.UploadToCloud(resourceDTO.UploadedImages, entityType);

                if (images != null && images.Any())
                {
                    var resourceImages = images.OfType<ResourceImage>().ToList();
                    resource.Images = resourceImages;
                }
            }
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

            if (ResourceWithDataDTO.RegionId != null)
            {
                var RegionExists = await CheckRegionExists(ResourceWithDataDTO.RegionId ?? default(int));
                if (!RegionExists) return CustomResult("Region Doesn't Exists", HttpStatusCode.NotFound);
            }


            var resource = _mapper.Map<Resource>(ResourceWithDataDTO);
            var result = await _resourceRepo.AddAsync(resource);

            (IActionResult ResourceDataValidation, List<ResourceData> resourceData) = await CheckResourceData(result, ResourceWithDataDTO.ResourceAttributes);
            if (ResourceDataValidation != null)
                return ResourceDataValidation;

            var res = await _resourceDataRepo.AddRangeAsync(resourceData);

            var resDTO = _mapper.Map<List<ResourceDataRespDTO>>(res);

            return CustomResult(resDTO);
        }





        [HttpPut("{id:int}")]
        public IActionResult Edit(int id,[FromBody] Decimal price,int? RegionId)
        {
            // Where the rest of the edit?
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
        private async Task<bool> CheckRegionExists(int RegionId)
        {
            if (RegionId == 0) return false;
            var result = await _regionRepo.GetByIdAsync(RegionId);
            if (result == null) return false;
            return true;
        }
    }
}
