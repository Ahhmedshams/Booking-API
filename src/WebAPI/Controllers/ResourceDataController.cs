using Application.Common.Interfaces.Repositories;
using Application.Common.Model;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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

        public ResourceDataController(IMapper mapper, IResourceDataRepo resourceDataRepo, IResourceRepo resourceRepo, IResourceMetadataRepo resourceMetadataRepo)
        {
            _mapper = mapper;
            _resourceDataRepo = resourceDataRepo;
            _resourceRepo = resourceRepo;
            _resourceMetadataRepo = resourceMetadataRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Result = await _resourceDataRepo.GetAllData();
            if (Result.Count() == 0)
                return CustomResult("No Resource Data Are Available ", HttpStatusCode.NotFound);

            return CustomResult(Result);

        }
       

        [HttpGet("Resource/{id:int}")]
        public async Task<IActionResult> FindResourceData(int id)
        {
            AllResourceData Result = await _resourceDataRepo.GetAllReourceData(id);
            if (Result == null)
                return CustomResult($"No Resource Are Available With ID {id} ", HttpStatusCode.NotFound);


            return CustomResult(Result);
        }

        [HttpGet("Type/{id:int}")]
        public async Task<IActionResult> AllDataByResourecTypeId(int id)
        {
            var Result = await _resourceDataRepo.GetAllDataByType(id);
            if (Result == null)
                return CustomResult($"No Resource Are Available ResourceType ID {id} ", HttpStatusCode.NotFound);


            return CustomResult(Result);
        }


        

        [HttpPost("AddRange/{id:int}")]
        public async Task<IActionResult> AddRange(int id, ResourceDataRespIDValueDTO[] resourceDTO)
        {

            var IdResalt = CheckID(id, resourceDTO);
            if (IdResalt != null)
                return IdResalt;

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var resourceData = _mapper.Map<IEnumerable<ResourceData>>(resourceDTO);
            var res = await _resourceDataRepo.AddRangeAsync(resourceData);

            return CustomResult(res);
        }


        [HttpPost("AddOne/{id:int}")]
        public async Task<IActionResult> AddOne(int id, ResourceDataRespIDValueDTO resourceDTO)
        {

            var IdResalt = CheckID(id, resourceDTO);
            if (IdResalt != null)
                return IdResalt;

            var ResourceDataValidation = await CheckResourceData(id, resourceDTO);
            if (ResourceDataValidation != null)
                return ResourceDataValidation;


            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var resourceData = _mapper.Map<ResourceData>(resourceDTO);
            var res = await _resourceDataRepo.AddAsync(resourceData);

            return CustomResult(res);
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

            var resourceDTO = _mapper.Map<ResourceMetaRespDTO>(Data);

            return CustomResult(resourceDTO);
        }

        private IActionResult CheckID(int ResID, ResourceDataRespIDValueDTO[] resourceDTO)
        {
            var ResourceCheck = CheckResource(ResID);
            if (ResourceCheck != null)
                return ResourceCheck;

            var CheckId = resourceDTO.Where(res => res.ResourceId != ResID);

            if(CheckId.Count() != 0)
                return CustomResult($"Not All Resource ID Are Same ", HttpStatusCode.BadRequest);
            else
                return null;
        }

        private IActionResult CheckID(int ResID, ResourceDataRespIDValueDTO resourceDTO)
        {
            var ResourceCheck = CheckResource(ResID);

            if (ResourceCheck != null)
                return ResourceCheck;

            if (ResID != resourceDTO.ResourceId)
                return CustomResult($"Not All Resource ID Are Same ", HttpStatusCode.BadRequest);
            else
                return null;
        }

        private IActionResult CheckResource(int ResID)
        {
            var resourceType = _resourceRepo.IsExist(ResID);
            if (!resourceType)
                return CustomResult($"No Resource Are Available With id {ResID}", HttpStatusCode.NotFound);
            else
                return null;
        }

        private async Task<IActionResult> CheckResourceData(int ResID, ResourceDataRespIDValueDTO resourceDTO)
        {

            var  resource = await _resourceRepo.GetByIdAsync(ResID);

            var resourceMetaData = await _resourceMetadataRepo.FindAsync(Re => Re.ResourceTypeId == resource.ResourceTypeId);
            if(resourceMetaData.Count()==0)
                return  CustomResult($"No Resource Metadata Available for ResourceType ID {ResID}", HttpStatusCode.NotFound);

           
               List<ResourceMetadata> Attribute = resourceMetaData.Where(res => res.AttributeId == resourceDTO.AttributeId).ToList();
               if (Attribute.Count() == 0)
                        return CustomResult(message: $"No AttributeId Available With ID{resourceDTO.AttributeId}", HttpStatusCode.NotFound);
                
                switch (Attribute[0].AttributeType) 
                {
                    case "Number":

                        if (Int32.TryParse(resourceDTO.AttributeValue, out int value))
                            break;
                        else
                            return CustomResult(message: $"Not a Valid Value for Attribute Value ID{resourceDTO.AttributeId}", HttpStatusCode.BadRequest);
                        break;
                }

            return null;
            
        }

            


        

    }
}
