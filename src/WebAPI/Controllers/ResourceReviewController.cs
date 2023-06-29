using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System.Net;
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

        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;
        public ResourceReviewController(IMapper mapper, IAccountRepository accountRepo, IResourceRepo resourceRepo, IResourceReviewRepo resourceReviewRepo, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions)
        {
            this.mapper = mapper;
            this.accountRepo = accountRepo;
            this.resourceRepo = resourceRepo;
            this.resourceReviewRepo = resourceReviewRepo;
            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value;
        }


        [HttpPost]
        public async Task<IActionResult> AddReview(ResourceReviewDTO resourceReviewDTO )
        {
            if(!ModelState.IsValid)
                return CustomResult(ModelState,HttpStatusCode.BadRequest);

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
                return CustomResult($"{ex}",HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("Resource/{id:int}")]
        public async Task<IActionResult> GetResourceReview(int id)
        {

            var Resource = resourceRepo.IsExist(id);

            if (!Resource)
                return CustomResult($"No Resource Exist With Id {id}", HttpStatusCode.BadRequest);

            var result = await resourceReviewRepo.FindAsync(e=> e.ResourceId == id);
            if (result.Count() == 0)
                return CustomResult($"No Review Exist for Resource Id {id}", HttpStatusCode.BadRequest);


            var resultDTO = mapper.Map<List<ResourceReviewResDTO>>(result);
            return CustomResult(resultDTO);

        }

        [HttpGet("User/{id:Guid}")]
        public async Task<IActionResult> GetUserReview(string id , int? bookingId)
        {
            var User = await accountRepo.IsExistAsync(id);

            if (!User)
                return CustomResult($"No User Exist  With Id {id}",HttpStatusCode.BadRequest);
            IEnumerable<ResourceReview> result;
            if (bookingId == null)
                   result = await resourceReviewRepo.FindAsync(e => e.UserId == id);
            else
                result = await resourceReviewRepo.FindAsync(e => e.UserId == id && e.BookingId == bookingId);


            if (result.Count() == 0)
                return CustomResult($"No Review Exist for User Id {id}", HttpStatusCode.BadRequest);

            var resultDTO = mapper.Map<List<ResourceReviewResDTO>>(result);
            return CustomResult(resultDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var Review = await resourceReviewRepo.GetByIdAsync(id);

            if (Review == null)
                return CustomResult($"No Review Exist  With Id {id}", HttpStatusCode.BadRequest);

            var resultDTO = mapper.Map<ResourceReviewResDTO>(Review);
            return CustomResult(resultDTO);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return CustomResult($"No Review Type IS Available With id {id}", HttpStatusCode.BadRequest);

            bool result = await resourceReviewRepo.SoftDeleteAsync(id);
            if (!result)
                return CustomResult($"No Review is available with id {id}", HttpStatusCode.BadRequest);


            return CustomResult(HttpStatusCode.NoContent);
        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Edit(int id , ResourceReviewEditDTO resourceReviewDTO)
        {

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var ResourceReview = mapper.Map<ResourceReview>(resourceReviewDTO);


            var result = await resourceReviewRepo.Patch(id, ResourceReview);

            if(result == null)
                return CustomResult($"No Review is available with id {id}", HttpStatusCode.BadRequest);
            else
                 return CustomResult(result);
        }


    }
}
