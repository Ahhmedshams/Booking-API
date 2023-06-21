using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceTypeController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IResourceTypeRepo _resourceTypeRepo;
        private readonly UploadImage _uploadImage;

        public ResourceTypeController(IMapper mapper, IResourceTypeRepo resourceTypeRepo, UploadImage uploadImage)
        {
            _mapper = mapper;
            _resourceTypeRepo = resourceTypeRepo;
            this._uploadImage = uploadImage;
        }

        [HttpGet]
      //  [Authorize(Permissions.ResourceTypes.Index)]
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
        public async Task<IActionResult> Add(ResourceTypeDTO resTypeDto)
        {
            if (!ModelState.IsValid) 
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

           var Result = await _resourceTypeRepo.IsExistAsync(resTypeDto.Name);
            if (Result)
                return CustomResult("This Name already Exist", HttpStatusCode.BadRequest);

            ResourceType resourceType = _mapper.Map<ResourceType>(resTypeDto);

            if (resTypeDto.UploadedImages != null && resTypeDto.UploadedImages.Any())
            {
                var entityType = "ResourceTypeImage";
                var images = await _uploadImage.UploadToCloud(resTypeDto.UploadedImages, entityType);

                if (images != null && images.Any())
                {
                    var resourceTypeImages = images.OfType<ResourceTypeImage>().ToList();
                    resourceType.Images = resourceTypeImages;
                }
            }
            //esourceType resourceType = new() { Name = Name };
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

           bool result =  await _resourceTypeRepo.SoftDeleteAsync(id);
            if(!result)
                return CustomResult($"No ResourceType is available with id {id}", HttpStatusCode.BadRequest);


            return CustomResult( HttpStatusCode.NoContent);
        }

       
    }
}
