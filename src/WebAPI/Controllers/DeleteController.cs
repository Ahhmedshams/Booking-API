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
    public class DeleteController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IResourceRepo _resourceRepo;
        private readonly IResourceTypeRepo _resourceTypeRepo;
        private readonly IResourceDataRepo _dataRepo;
        private readonly IResourceMetadataRepo _resourceMetadataRepo;

        public DeleteController(IMapper mapper, 
                                        IResourceRepo resourceRepo, 
                                        IResourceTypeRepo resourceTypeRepo, 
                                        IResourceDataRepo dataRepo, 
                                        IResourceMetadataRepo resourceMetadataRepo)
        {
            _mapper = mapper;
            _resourceRepo = resourceRepo;
            _resourceTypeRepo = resourceTypeRepo;
            _dataRepo = dataRepo;
            _resourceMetadataRepo = resourceMetadataRepo;
        }


        [HttpDelete("Soft/ResourceType/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            if (id == 0)
                return CustomResult($"No Resource Type Is Available With id {id}", HttpStatusCode.BadRequest);

           var Type =  await _resourceTypeRepo.GetByIdAsync(id);
            if(Type== null)
                return CustomResult($"No Resource Type Is Available With id {id}", HttpStatusCode.BadRequest);


           var MetaData = await _resourceMetadataRepo.FindAsync(re=> re.ResourceTypeId == id);
           var Resource = await _resourceRepo.FindAsync(re => re.ResourceTypeId == id);


            _resourceTypeRepo.SoftDelete(Type);
            if(MetaData.Count() != 0)
                _resourceMetadataRepo.SoftDelete(MetaData);
            if(Resource.Count() != 0)
                 _resourceRepo.SoftDelete(Resource);
            await _resourceTypeRepo.SaveChangesAsync();

            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpDelete("Soft/ResourceAtt/{id:int}")]
        public async Task<IActionResult> SoftDeleteAttrib(int id)
        {
            if (id == 0)
                return CustomResult($"No Resource Attribute Is Available With id {id}", HttpStatusCode.BadRequest);

            var Attribute = await _resourceMetadataRepo.GetByIdAsync(id);
            if (Attribute == null)
                return CustomResult($"No Resource Attribute Is Available With id {id}", HttpStatusCode.BadRequest);


            var Data = await _dataRepo.FindAsync(re => re.AttributeId == id);

            _resourceMetadataRepo.SoftDelete(Attribute);
            _dataRepo.SoftDelete(Data);
            await _resourceTypeRepo.SaveChangesAsync();

            return CustomResult(HttpStatusCode.NoContent);
        }

        [HttpDelete("Soft/Resource/{id:int}")]
        public async Task<IActionResult> SoftDeleteResource(int id)
        {
            if (id == 0)
                return CustomResult($"No Resource Available With id {id}", HttpStatusCode.BadRequest);

            var Resource = await _resourceRepo.GetByIdAsync(id);
            if (Resource == null)
                return CustomResult($"No Resource  Available With id {id}", HttpStatusCode.BadRequest);


            var Data = await _dataRepo.FindAsync(re => re.ResourceId == id);

            _resourceRepo.SoftDelete(Resource);
            _dataRepo.SoftDelete(Data);
            await _resourceTypeRepo.SaveChangesAsync();

            return CustomResult(HttpStatusCode.NoContent);
        }


    }
}
