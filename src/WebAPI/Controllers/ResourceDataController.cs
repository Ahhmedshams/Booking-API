using Application.Common.Model;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceDataController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IResourceDataRepo _resourceDataRepo;
        private readonly IResourceRepo _resourceRepo;
        private readonly IResourceMetadataRepo _resourceMetadataRepo;

        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;

        public ResourceDataController(IMapper mapper, IResourceDataRepo resourceDataRepo, IResourceRepo resourceRepo, IResourceMetadataRepo resourceMetadataRepo, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions)
        {
            _mapper = mapper;
            _resourceDataRepo = resourceDataRepo;
            _resourceRepo = resourceRepo;
            _resourceMetadataRepo = resourceMetadataRepo;

            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SieveModel sieveModel)
        {
            var Result = await _resourceDataRepo.GetAllData();
            //if (Result.Count() == 0)
            //    return CustomResult("No Resource Data Are Available ", HttpStatusCode.NotFound);

            var FilteredResource = _sieveProcessor.Apply(sieveModel, Result.AsQueryable());
            return CustomResult(FilteredResource);

        }
       
        [HttpGet("Resource/{id:int}")]
        public async Task<IActionResult> FindResourceData(int id)
        {
            AllResourceData Result = await _resourceDataRepo.GetAllReourceData(id);
            //if (Result == null)
            //    return CustomResult($"No Resource Are Available With ID {id} ", HttpStatusCode.NotFound);

            return CustomResult(Result);
        }

        [HttpGet("Type/{id:int}")]
        public async Task<IActionResult> AllDataByResourecTypeId(int id)
        {
            var Result = await _resourceDataRepo.GetAllDataByType(id);
            //if (Result == null)
            //    return CustomResult($"No Resource Are Available ResourceType ID {id} ", HttpStatusCode.NotFound);


            return CustomResult(Result);
        }

        [HttpPost("AddRange/{id:int}")]
        public async Task<IActionResult> AddRange(int id, ResourceDataRespIDValueDTO[] resourceDTO)
        {

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

             (IActionResult ResourceDataValidation , List < ResourceData > resourceData) = await CheckResourceData(id, resourceDTO);
            if (ResourceDataValidation != null)
                return ResourceDataValidation;

            var res = await _resourceDataRepo.AddRangeAsync(resourceData);

            var resDTO = _mapper.Map<ResourceDataRespDTO>(res);

            return CustomResult(resDTO);
        }


        [HttpPost("AddOne/{id:int}")]
        public async Task<IActionResult> AddOne(int id, ResourceDataRespIDValueDTO resourceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var ResourceDataValidation = await CheckResourceData(id, resourceDTO);
            if (ResourceDataValidation != null)
                return ResourceDataValidation;


            ResourceData resourceData = new()
            {
                ResourceId = id,
                AttributeId = resourceDTO.AttributeId,
                AttributeValue = resourceDTO.AttributeValue
            };
            var res = await _resourceDataRepo.AddAsync(resourceData);

            var resDTO = _mapper.Map<ResourceDataRespDTO>(res);

            return CustomResult(resDTO);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, ResourceDataRespIDValueDTO resourceAttribute)
        {
            var Data = await _resourceDataRepo.FindAsync( id, resourceAttribute.AttributeId);
            if (Data == null)
                return CustomResult($"No Resource Metadata Available With id {id}", HttpStatusCode.NotFound);

            if(!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);


            Data.AttributeValue = resourceAttribute.AttributeValue;

           await _resourceDataRepo.SaveChangesAsync();

            var resourceDTO = _mapper.Map<ResourceDataRespDTO>(Data);

            return CustomResult(resourceDTO);
        }

        private async Task<IActionResult> CheckResourceData(int ResID, ResourceDataRespIDValueDTO resourceDTO)
        {
            //Check If Resource Exist 
            var  resource = await _resourceRepo.GetByIdAsync(ResID);
            if (resource==null)
                return CustomResult($"No Resource Available for Resource ID {ResID}", HttpStatusCode.NotFound);

            //Get Resource Type MetaData
            var resourceMetaData = await _resourceMetadataRepo.FindAsync(Re => Re.ResourceTypeId == resource.ResourceTypeId);
            if(resourceMetaData.Count()==0)
                return  CustomResult($"No Resource Metadata Available for ResourceType ID {ResID}", HttpStatusCode.NotFound);



            //Check If Attribute Id Exist
            ResourceMetadata Attribute = resourceMetaData.FirstOrDefault(res => res.AttributeId == resourceDTO.AttributeId);
               if (Attribute==null)
                        return CustomResult(message: $"No AttributeId Available With ID{resourceDTO.AttributeId}", HttpStatusCode.NotFound);

            //Check If Attribute Id and Resource ID Inserted Before 
            bool IsExist = await _resourceDataRepo.IsExistAsync(res => res.AttributeId == resourceDTO.AttributeId && res.ResourceId == ResID);
            if (IsExist)
                return CustomResult($"The Attribute with ID {resourceDTO.AttributeId} Already inserted before", HttpStatusCode.NotFound);

            //Validate Attribute value
            var ValueResult = ValidateValue(Attribute.AttributeType, resourceDTO.AttributeValue, resourceDTO.AttributeId);

            if (ValueResult != null)
                return ValueResult;

            return null;
            
        }

        private async Task<(IActionResult, List<ResourceData>)> CheckResourceData(int ResID, ResourceDataRespIDValueDTO[] resourceDTO )
        {

            List<ResourceData> resourceDatas = new List<ResourceData>();
           //Check If Resource Exist 
           var resource = await _resourceRepo.GetByIdAsync(ResID);
            if (resource == null)
                return ( CustomResult($"No Resource Available for Resource ID {ResID}", HttpStatusCode.NotFound), null );

            //Get Resource Type MetaData
            var resourceMetaData = await _resourceMetadataRepo.FindAsync(Re => Re.ResourceTypeId == resource.ResourceTypeId);
            if (resourceMetaData.Count() == 0)
                return ( CustomResult($"No Resource Metadata Available for ResourceType ID {ResID}", HttpStatusCode.NotFound) , null );

            foreach ( var item in resourceDTO )
            {
                //Check If Attribute Id Exist
                ResourceMetadata Attribute = resourceMetaData.FirstOrDefault(res => res.AttributeId == item.AttributeId);
                if (Attribute == null)
                    return ( CustomResult(message: $"No AttributeId Available With ID {item.AttributeId}", HttpStatusCode.NotFound) , null );

                //Check If Attribute Id and Resource ID Inserted Before 
                bool IsExist =  await _resourceDataRepo.IsExistAsync(res=> res.AttributeId == item.AttributeId && res.ResourceId == ResID);
                if(IsExist)
                    return ( CustomResult($"The Attribute with ID {item.AttributeId} Already inserted before", HttpStatusCode.NotFound), null);

                
                //Validate Attribute value
                var ValueResult = ValidateValue(Attribute.AttributeType, item.AttributeValue, item.AttributeId);

                if (ValueResult != null)
                    return (ValueResult, null);

                ResourceData resourceData = new()
                {
                    ResourceId = ResID,
                    AttributeId = item.AttributeId,
                    AttributeValue = item.AttributeValue
                };

                resourceDatas.Add(resourceData);
            }

            return (null , resourceDatas);

        }

        private IActionResult ValidateValue(string Type , string Value , int AttId)
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
