using AutoMapper;
using CoreApiResponse;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceReviewController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAccountRepository accountRepo;
        private readonly IResourceRepo resourceRepo;
        private readonly IResourceReviewRepo resourceReviewRepo;

        public ResourceReviewController(IMapper mapper, IAccountRepository accountRepo, IResourceRepo resourceRepo, IResourceReviewRepo resourceReviewRepo)
        {
            this.mapper = mapper;
            this.accountRepo = accountRepo;
            this.resourceRepo = resourceRepo;
            this.resourceReviewRepo = resourceReviewRepo;
        }


        [HttpPost]
        public async Task<IActionResult> AddReview(ResourceReviewDTO resourceReviewDTO )
        {
            if(!ModelState.IsValid)
                return CustomResult(ModelState,System.Net.HttpStatusCode.BadRequest);

            var User = await accountRepo.IsExistAsync(resourceReviewDTO.UserId);
            var Resource =  resourceRepo.IsExist(resourceReviewDTO.ResourceId);

            if(!User)
                return CustomResult($"No User Exist With Id {resourceReviewDTO.UserId}",System.Net.HttpStatusCode.BadRequest);
            if (!Resource)
                return CustomResult($"No Resource Exist With Id {resourceReviewDTO.ResourceId}", System.Net.HttpStatusCode.BadRequest);

            var ResourceReview =  mapper.Map<ResourceReview>(resourceReviewDTO);
            try
            {
               var result = await resourceReviewRepo.AddAsync(ResourceReview);
               var resultDTO = mapper.Map<ResourceReviewResDTO>(result);
               return CustomResult(resultDTO);
            }
            catch (Exception ex)
            {
                return CustomResult($"{ex}", System.Net.HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("Resource/{id:int}")]
        public async Task<IActionResult> GetResourceReview(int id)
        {

            var Resource = resourceRepo.IsExist(id);

            if (!Resource)
                return CustomResult($"No Resource Exist With Id {id}", System.Net.HttpStatusCode.BadRequest);

            var result = await resourceReviewRepo.FindAsync(e=> e.ResourceId == id);
            if(result.Count() == 0)
                return CustomResult($"No Review Exist for Resource Id {id}", System.Net.HttpStatusCode.BadRequest);


            var resultDTO = mapper.Map<List<ResourceReviewResDTO>>(result);
            return CustomResult(resultDTO);

        }

        [HttpGet("User/{id:Guid}")]
        public async Task<IActionResult> GetResourceReview(string id)
        {
            var User = await accountRepo.IsExistAsync(id);

            if (!User)
                return CustomResult($"No User Exist  With Id {id}", System.Net.HttpStatusCode.BadRequest);

            var result = await resourceReviewRepo.FindAsync(e => e.UserId == id);

            if (result.Count() == 0)
                return CustomResult($"No Review Exist for User Id {id}", System.Net.HttpStatusCode.BadRequest);

            var resultDTO = mapper.Map<List<ResourceReviewResDTO>>(result);
            return CustomResult(resultDTO);
        }


    }
}
