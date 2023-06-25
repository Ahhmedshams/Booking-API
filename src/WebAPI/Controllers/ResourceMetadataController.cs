using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.DTO;
using WebAPI.Utility;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceMetadataController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IResourceMetadataRepo _resourceMetadataRepo;
        private readonly IResourceTypeRepo _resourceTypeRepo;


        public ResourceMetadataController(IMapper mapper,
                                        IResourceMetadataRepo resourceMetadataRepo,
                                        IResourceTypeRepo resourceTypeRepo)
        {
            _mapper = mapper;
            _resourceMetadataRepo = resourceMetadataRepo;
            _resourceTypeRepo = resourceTypeRepo;
        }

        [HttpPost("AddRange/{ResTypeID:int}")]
        public async Task<IActionResult> AddRange(int ResTypeID, ResourceMetaReqDTO[] resourceMetaDTO)
        {
            var IdResalt = Check(ResTypeID, resourceMetaDTO);
            if (IdResalt != null)
                return IdResalt;
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var ValidResourceMetaData = await ValidAttribute(resourceMetaDTO, ResTypeID);

            var resourceType = _mapper.Map<IEnumerable<ResourceMetadata>>(ValidResourceMetaData);
            var res = await _resourceMetadataRepo.AddRangeAsync(resourceType);
            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(res);

            return CustomResult(resourceDTO);
        }

        [HttpPost("AddOne/{ResTypeID:int}")]
        public async Task<IActionResult> AddOne(int ResTypeID, ResourceMetaReqDTO resourceMetaDTO)
        {
            var IdResult = await Check(ResTypeID, resourceMetaDTO);
            if (IdResult != null)
                return IdResult;

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);


            var resourceType = _mapper.Map<ResourceMetadata>(resourceMetaDTO);
            var res = await _resourceMetadataRepo.AddAsync(resourceType);

            return CreatedAtAction("GetById", new { id = res.AttributeId }, res);
        }


        [HttpPost("/api/Create/ResourceType/{Name:alpha}")]
        public async Task<IActionResult> CreateFullResourceType(string Name, ResourceAttribute[] resourceAttribute)
        {
            var Result = await _resourceTypeRepo.IsExistAsync(Name);
            if (Result)
                return CustomResult("This Name already Exist", HttpStatusCode.BadRequest);

            var NewResourceType = new ResourceType() { Name = Name };
            var type = await _resourceTypeRepo.AddAsync(NewResourceType);

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            List<ResourceMetadata> ResourcesMetaData = ConvertToResourceMetadata(type.Id, resourceAttribute);
            var res = await _resourceMetadataRepo.AddRangeAsync(ResourcesMetaData);
            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(res);

            return CustomResult(resourceDTO);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<ResourceMetadata> resource = await _resourceMetadataRepo.GetAllAsync();
            //if (resource.Count() == 0)
            //    return CustomResult("No Resource Metadata Are Available ", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            ResourceMetadata resource = await _resourceMetadataRepo.GetByIdAsync(id);
            //if (resource == null)
            //    return CustomResult($"No Resource Metadata Available With id {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<ResourceMetaRespDTO>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, ResourceAttribute resourceAttribute)
        {
            ResourceMetadata resource = await _resourceMetadataRepo.GetByIdAsync(id);
            if (resource == null)
                return CustomResult($"No Resource Metadata Available With id {id}", HttpStatusCode.NotFound);

            ResourceMetadata resourceMetadata = _mapper.Map<ResourceMetadata>(resourceAttribute);

            var result = await _resourceMetadataRepo.EditAsync(id, resourceMetadata, Res => Res.AttributeId);

            var resourceDTO = _mapper.Map<ResourceMetaRespDTO>(result);

            return CustomResult(resourceDTO);
        }

        [HttpGet("/api/ResourceAttribute/{id:int}")]
        public async Task<IActionResult> GetByResourceTypeId(int id)
        {
            var resource = await _resourceMetadataRepo.FindAsync(Re => Re.ResourceTypeId == id);
            //if (resource.Count() == 0)
            //    return CustomResult($"No Resource Metadata Available To Resource Type Id= {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(resource);
            return CustomResult(resourceDTO);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Result = await _resourceTypeRepo.SoftDeleteAsync(id);
            if (!Result)
                return CustomResult($"No Resource Metadata Available With id {id}", HttpStatusCode.NotFound);
            else
                return CustomResult(HttpStatusCode.NoContent);
        }

        private async Task<IActionResult> Check(int ResTypeID, ResourceMetaReqDTO resourceMetaDTO)
        {

            var TypeCheck = CheckResourceType(ResTypeID);
            if (TypeCheck != null)
                return TypeCheck;

            if (ResTypeID == 0 || ResTypeID != resourceMetaDTO.ResourceTypeId)
                return CustomResult($"Error In Resource Type {ResTypeID}", HttpStatusCode.BadRequest);

            var CheckAttribute = await CheckAttributeName(resourceMetaDTO);
            if (CheckAttribute != null)
                return CheckAttribute;

            return null;
        }

        private IActionResult Check(int ResTypeID, ResourceMetaReqDTO[] ResourceMetaDTO)
        {
            var TypeCheck = CheckResourceType(ResTypeID);
            if (TypeCheck != null)
                return TypeCheck;

            var CheckId = ResourceMetaDTO.Where(Res => Res.ResourceTypeId != ResTypeID);

            if (CheckId.Count() != 0)
                return CustomResult($"Not All ResourceType ID Are Same ", HttpStatusCode.BadRequest);


            return null;
        }

        private IActionResult CheckResourceType(int ResTypeID)
        {
            var resourceType = _resourceTypeRepo.IsExist(ResTypeID);
            if (!resourceType)
                return CustomResult($"No Resource Type Are Available With id {ResTypeID}", HttpStatusCode.NotFound);
            else
                return null;
        }

        private List<ResourceMetadata> ConvertToResourceMetadata(int id, ResourceAttribute[] resourceAttribute)
        {
            var result = new List<ResourceMetadata>();
            var ValidAttributes = resourceAttribute.CheckIsValidAttribute();

            foreach (ResourceAttribute item in ValidAttributes)
            {
                var ResourceMeta = item.ToResourceMetadata(id);
                result.Add(ResourceMeta);
            }
            return result;
        }

        private async Task<IActionResult> CheckAttributeName(ResourceMetaReqDTO resourceMetaReqDTO)
        {
            var Result = await _resourceMetadataRepo.FindAsync(res => res.AttributeName == resourceMetaReqDTO.AttributeName
                                              && res.ResourceTypeId == resourceMetaReqDTO.ResourceTypeId
                                              );

            if (Result.Count() != 0)
                return CustomResult("This Attribute Name Is Dublicated", HttpStatusCode.BadRequest);
            else
                return null;
        }
        private async Task<List<ResourceMetaReqDTO>> ValidAttribute(ResourceMetaReqDTO[] resourceMetaReqDTO,int ResTypeID)
        {
            List<ResourceMetaReqDTO> ValidResMetadata = new();
            var ExistedMetaData = await _resourceMetadataRepo.FindAsync( res=> res.ResourceTypeId == ResTypeID);

            foreach (var item in resourceMetaReqDTO)
            {
               var Result =  ExistedMetaData.Any(res=> res.AttributeName ==  item.AttributeName);

                if(!Result)
                    ValidResMetadata.Add(item);
            }

            return ValidResMetadata;
            
        }

    }
}
