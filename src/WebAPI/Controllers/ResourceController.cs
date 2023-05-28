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
        private readonly IResourceRepo _resourceRepo;
        private readonly IResourceTypeRepo _resourceTypeRepo;
        

       
        public ResourceController(IMapper mapper,  IResourceRepo resourceRepo, IResourceTypeRepo resourceTypeRepo)
        {
            _mapper = mapper;
            _resourceRepo = resourceRepo;
            _resourceTypeRepo = resourceTypeRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Resource> resource = await _resourceRepo.GetAllAsync();
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


       


        [HttpPut("{id:int}")]
        public IActionResult Edit(int id,[FromBody] Decimal price)
        {
            if (price <= 0)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var result = _resourceRepo.EditPrice(id, price);

            return CustomResult(result);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            if (id == 0)
                return CustomResult($"No Resource Is Available With id {id}", HttpStatusCode.BadRequest);

           await  _resourceRepo.DeleteAsync(id);

            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpDelete("SoftDelete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            if (id == 0)
                return CustomResult($"No Resource Is Available With id {id}", HttpStatusCode.BadRequest);

            await _resourceRepo.SoftDeleteAsync(id);

            return CustomResult(HttpStatusCode.NoContent);
        }
    }
}
