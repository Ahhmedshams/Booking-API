using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
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


        public ResourceDataController(IMapper mapper, IResourceDataRepo resourceDataRepo, IResourceRepo resourceRepo)
        {
            _mapper = mapper;
            _resourceDataRepo = resourceDataRepo;
            _resourceRepo = resourceRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var ResourceData = _resourceDataRepo.GetAll();
            if (ResourceData.Count() == 0)
                return CustomResult("No Resource Data Are Available ", HttpStatusCode.NotFound);
            var Result = _mapper.Map<List<ResourceDataRespIDValueDTO>>(ResourceData);

            return CustomResult(Result);
             
        }

        [HttpGet("GetResourceData")]
        public IActionResult FindResourceData(int resourceId)
        [HttpGet("GetResourceData/{id:int}")]
        public IActionResult FindResourceData(int id)
        {
           var ResourceData =_resourceDataRepo.Find(res=> res.ResourceId == resourceId);
           var ResourceData =_resourceDataRepo.Find(res=> res.ResourceId == id);

            if (ResourceData.Count() == 0)
                return CustomResult("No Resource data Are Available ", HttpStatusCode.NotFound);

            var Result = _mapper.Map<List<ResourceDataRespIDValueDTO>>(ResourceData);

            return CustomResult(Result);
        }


        [HttpPost("AddRange/{id:int}")]
        public IActionResult AddRange(int id, ResourceDataRespIDValueDTO[] resourceDTO)
        {

            var IdResalt = CheckID(id, resourceDTO);
            if (IdResalt != null)
                return IdResalt;

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var resourceData = _mapper.Map<IEnumerable<ResourceData>>(resourceDTO);
            var res = _resourceDataRepo.AddRange(resourceData);

            return CustomResult(res);
        }


        [HttpPost("AddOne/{id:int}")]
        public IActionResult AddOne(int id, ResourceDataRespIDValueDTO resourceDTO)
        {

            var IdResalt = CheckID(id, resourceDTO);
            if (IdResalt != null)
                return IdResalt;

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var resourceData = _mapper.Map<ResourceData>(resourceDTO);
            var res = _resourceDataRepo.Add(resourceData);

            return CustomResult(res);
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
                return CustomResult($"No Resource  Are Available With id {ResID}", HttpStatusCode.NotFound);
            else
                return null;
        }


    }
}
