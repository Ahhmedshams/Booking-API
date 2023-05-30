using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
            var IdResalt = CheckID(ResTypeID, resourceMetaDTO);
            if (IdResalt != null)
                return IdResalt;
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

           var resourceType = _mapper.Map<IEnumerable<ResourceMetadata>>(resourceMetaDTO);
           var res = await _resourceMetadataRepo.AddRangeAsync(resourceType);
           var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(res);

            return CustomResult(resourceDTO);
        }

        [HttpPost("AddOne/{ResTypeID:int}")]
        public async Task<IActionResult> AddOne(int ResTypeID, ResourceMetaReqDTO resourceMetaDTO)
        {
            var IdResult = CheckID(ResTypeID, resourceMetaDTO);
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
            var NewResourceType = new ResourceType() { Name = Name };
            var type =  await _resourceTypeRepo.AddAsync(NewResourceType);

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

           var Convertres= ConvertToResourceMetadata(type.Id, resourceAttribute);
            var res = await _resourceMetadataRepo.AddRangeAsync(Convertres);
            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(res);

            return CustomResult(resourceDTO);
        }



        [HttpGet]
        public  async Task<IActionResult> GetAll()
        {
            IEnumerable<ResourceMetadata> resource = await _resourceMetadataRepo.GetAllAsync();
            if (resource.Count() == 0)
                return CustomResult("No Resource Metadata Are Available ", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            ResourceMetadata resource = await _resourceMetadataRepo.GetByIdAsync(id);
            if (resource == null)
                return CustomResult($"No Resource Metadata Available With id {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<ResourceMetaRespDTO>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id , ResourceAttribute resourceAttribute)
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
            var resource = await _resourceMetadataRepo.FindAsync(Re=> Re.ResourceTypeId == id);
            if (resource.Count() ==0)
                return CustomResult($"No Resource Metadata Available To Resource Type Id= {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }


        private IActionResult CheckID(int ResTypeID, ResourceMetaReqDTO resourceMetaDTO)
        {

            var TypeCheck = CheckResourceType(ResTypeID);
            if (TypeCheck != null)
                return TypeCheck;

            if (ResTypeID == 0 || ResTypeID != resourceMetaDTO.ResourceTypeId)
                return CustomResult($"Error In Resource Type {ResTypeID}", HttpStatusCode.BadRequest);

            return null;
        }

        private IActionResult CheckID(int ResTypeID, ResourceMetaReqDTO[] ResourceMetaDTO)
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
            

            foreach (ResourceAttribute item in resourceAttribute)
            {
              var ResourceMeta =  item.ToResourceMetadata(id);
                result.Add(ResourceMeta);
            }
            return result;
        }

    }
}
