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
        public IActionResult GetAll()
        {
            IEnumerable<ResourceType> resourceTypes = _resourceTypeRepo.GetAll();
            if (resourceTypes.Count() == 0 ) 
                return CustomResult("No Resource Type Are Available", HttpStatusCode.NotFound);
            
            List<ResourceTypeDTO> resourceTypesDTO = _mapper.Map<List<ResourceTypeDTO>>(resourceTypes);

            return CustomResult(resourceTypesDTO);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            ResourceType resourceType = _resourceTypeRepo.GetById(id);
            if (resourceType == null)
                return CustomResult($"No Resource Type Are Available With id {id}", HttpStatusCode.NotFound);

            ResourceTypeDTO resourceTypeDTO = _mapper.Map<ResourceTypeDTO>(resourceType);

            return CustomResult(resourceTypeDTO);
        }

        [HttpPost]
        public IActionResult Add(ResourceTypeDTO resourceTypeDTO)
        {
            if (!ModelState.IsValid) 
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            
            ResourceType resourceType = _mapper.Map<ResourceType>(resourceTypeDTO);
            _resourceTypeRepo.Add(resourceType);
            

            return CustomResult(resourceTypeDTO);
        }


        [HttpPut("{id:int}")]
        public IActionResult Edit( int id , ResourceTypeDTO resourceTypeDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            ResourceType resourceType = _mapper.Map<ResourceType>(resourceTypeDTO);
            _resourceTypeRepo.Edit(id,resourceType,Res=>Res.Id);

            return CustomResult(resourceTypeDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteById(int id)
        {
            if (id == 0 ) 
              return  CustomResult($"No Resource Type Are Available With id {id}", HttpStatusCode.BadRequest);

            _resourceTypeRepo.Delete(id);

            return CustomResult( HttpStatusCode.NoContent);
        }
    }
}
