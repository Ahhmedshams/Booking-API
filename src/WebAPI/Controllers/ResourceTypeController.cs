using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceTypeController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IResourceTypeRepo _resourceTypeRepo;

        public ResourceTypeController(IMapper mapper, IResourceTypeRepo resourceTypeRepo)
        {
            _mapper = mapper;
            _resourceTypeRepo = resourceTypeRepo;
        }

        [HttpGet]
        public async Task< IActionResult> GetAll()
        {
            IEnumerable<ResourceType> resourceTypes = await _resourceTypeRepo.GetAllAsync();
            if (resourceTypes.Count() == 0 ) 
                return CustomResult("No Resource Type Are Available", HttpStatusCode.NotFound);
            
            List<ResourceTypeDTO> resourceTypesDTO = _mapper.Map<List<ResourceTypeDTO>>(resourceTypes);

            return CustomResult(resourceTypesDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            ResourceType resourceType = await _resourceTypeRepo.GetByIdAsync(id);
            if (resourceType == null)
                return CustomResult($"No Resource Type Are Available With id {id}", HttpStatusCode.NotFound);

            ResourceTypeDTO resourceTypeDTO = _mapper.Map<ResourceTypeDTO>(resourceType);

            return CustomResult(resourceTypeDTO);
        }


        [HttpPost]
        public async Task<IActionResult> Add(ResourceTypeDTO resourceTypeDTO)
        {
            if (!ModelState.IsValid) 
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            
            ResourceType resourceType = _mapper.Map<ResourceType>(resourceTypeDTO);
             await _resourceTypeRepo.AddAsync(resourceType);

            return CreatedAtAction("GetById", new { id = resourceType.Id }, resourceType);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit( int id , ResourceTypeDTO resourceTypeDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            ResourceType resourceType = _mapper.Map<ResourceType>(resourceTypeDTO);
           var result = await _resourceTypeRepo.EditAsync(id, resourceType, Res => Res.Id);

            return CustomResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            if (id == 0 ) 
              return  CustomResult($"No Resource Type IS Available With id {id}", HttpStatusCode.BadRequest);

           await _resourceTypeRepo.DeleteAsync(id);

            return CustomResult( HttpStatusCode.NoContent);
        }

       
    }
}
