using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
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
        private readonly ApplicationDbContext _context;
        private readonly IResourceRepo _resourceRepo;

        public ResourceController(IMapper mapper, ApplicationDbContext context, IResourceRepo resourceRepo)
        {
            _mapper = mapper;
            _context = context;
            _resourceRepo = resourceRepo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Resource> resource = _resourceRepo.GetAll();
            if (resource.Count() == 0)
                return CustomResult("No Resource Are Available", HttpStatusCode.NotFound);

            List<ResourceRespDTO> resourceDTO = _mapper.Map<List<ResourceRespDTO>>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            Resource resource = _resourceRepo.GetById(id);
            if (resource == null)
                return CustomResult($"No Resource Type Are Available With id {id}", HttpStatusCode.NotFound);

            var resourceDTO = _mapper.Map<ResourceRespDTO>(resource);

            return CustomResult(resourceDTO);
        }

        [HttpPost]
        public IActionResult Add(ResourceReqDTO resourceDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var resource = _mapper.Map<Resource>(resourceDTO);
            var result =  _resourceRepo.Add(resource);


            return CustomResult(result);
        }


        //[HttpPut("{id:int}")]
        //public IActionResult Edit(int id, ResourceReqDTO resourceDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return CustomResult(ModelState, HttpStatusCode.BadRequest);

        //    var resource = _mapper.Map<Resource>(resourceDTO);
        //   var result = _resourceRepo.Edit(id, resource, Res => Res.Id);

        //    return CustomResult(result);
        //}


        [HttpPut("{id:int}")]
        public IActionResult Edit(int id,[FromBody] Decimal price)
        {
            if (price <= 0)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var result = _resourceRepo.EditPrice(id, price);

            return CustomResult(result);
        }
    }
}
