using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpPost]
        public IActionResult AddRange(ResourceMetaReqDTO[] resourceMetaDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

           var resourceType = _mapper.Map<IEnumerable<ResourceMetadata>>(resourceMetaDTO);
           var res = _resourceMetadataRepo.AddRange(resourceType);

            return CustomResult(res);
        }

        [HttpPost("ResTypeID:int")]
        public IActionResult AddOne(int ResTypeID, ResourceMetaReqDTO resourceMetaDTO)
        {
            var IdResalt = CheckID(ResTypeID, resourceMetaDTO);
            if (IdResalt != null)
                return IdResalt;

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);


            var resourceType = _mapper.Map<ResourceMetadata>(resourceMetaDTO);
            var res =_resourceMetadataRepo.Add(resourceType);

            return CustomResult(res);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<ResourceMetadata> resource = _resourceMetadataRepo.GetAll();
            if (resource.Count() == 0)
                return CustomResult("No Resource Metadata Are Available ", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }


        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            ResourceMetadata resource = _resourceMetadataRepo.GetById(id);
            if (resource == null)
                return CustomResult($"No Resource Metadata Available With id {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<ResourceMetaRespDTO>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpGet("GetResourceAttribute")]
        public IActionResult GetByResourceTypeId([FromQuery] int TypeId)
        {
            var resource = _resourceMetadataRepo.Find(Re=> Re.ResourceTypeId == TypeId);
            if (resource.Count() ==0)
                return CustomResult($"No Resource Metadata Available To Resource Type Id= {TypeId}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<List<ResourceMetaRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }


        private IActionResult CheckID(int ResTypeID, ResourceMetaReqDTO resourceMetaDTO)
        {
            if (ResTypeID == 0 || ResTypeID != resourceMetaDTO.ResourceTypeId)
                return CustomResult($"Error In Resource Type {ResTypeID}", HttpStatusCode.BadRequest);

            var resourceType = _resourceTypeRepo.IsExist(ResTypeID);
            if (!resourceType)
                return CustomResult($"No Resource Type Are Available With id {ResTypeID}", HttpStatusCode.NotFound);

            return null;
        }


    }
}
