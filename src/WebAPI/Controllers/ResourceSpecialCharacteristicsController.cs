﻿using AutoMapper;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Application.Common.Interfaces.Repositories;
using CoreApiResponse;
using Domain.Entities;
using Org.BouncyCastle.Asn1.Cmp;
using System.Linq.Expressions;
using System.Security.Cryptography;
using WebAPI.Profiles;
using Sieve.Models;
using Sieve.Services;
using Microsoft.Extensions.Options;
using Stripe;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceSpecialCharacteristicsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IResourceRepo _resourceRepo;
        private readonly IScheduleItemRepo _scheduleItemRepo;
        private readonly IResourceSpecialCharacteristicsRepo _resourceSpecialCharacteristicsRepository;

        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;
        //private ResourceSpecialCharacteristicsRepository _resourceSpecialCharacteristicsRepository = new ResourceSpecialCharacteristicsRepository();
        public ResourceSpecialCharacteristicsController(
            IMapper mapper,
            IResourceRepo resourceRepo,
            IScheduleItemRepo scheduleItemRepo,
            IResourceSpecialCharacteristicsRepo resourceSpecialCharacteristicsRepo,
            ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions
            )
        {
            _mapper = mapper;
            _resourceRepo = resourceRepo;
            _scheduleItemRepo = scheduleItemRepo;
            _resourceSpecialCharacteristicsRepository = resourceSpecialCharacteristicsRepo;
            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SieveModel sieveModel)
        {
            IEnumerable<ResourceSpecialCharacteristics> resourceSpecialCharacteristics =
                await _resourceSpecialCharacteristicsRepository.GetAllAsync(
                    includes: new Expression<Func<ResourceSpecialCharacteristics, object>>[]
                    {
                        r => r.ScheduleItem,
                        r => r.Resource
                    }
                );

            if (!resourceSpecialCharacteristics.Any())
            {
                return Ok(Enumerable.Empty<ResourceSpecialCharacteristics>());
            }

            //List<ResourceSpecialCharacteristicsDTO> resourceSpecialCharacteristicsDTOs =
            //resourceSpecialCharacteristics._mapper.Map<ResourceSpecialCharacteristicsDTO>;

            List<ResourceSpecialCharacteristicsDTO> resourceSpecialCharacteristics1 =
                _mapper.Map<List<ResourceSpecialCharacteristicsDTO>>(resourceSpecialCharacteristics);

            var FilteredRSC = _sieveProcessor.Apply(sieveModel, resourceSpecialCharacteristics1.AsQueryable());

            return CustomResult(FilteredRSC);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            ResourceSpecialCharacteristics resourceSpecialCharacteristics = await _resourceSpecialCharacteristicsRepository.GetByIdAsync(id);
            //if(resourceSpecialCharacteristics ==null)
            //{
            //    return StatusCode((int)HttpStatusCode.NotFound, $"No Resource Type Are Available With id {id}");
            //}
            return CustomResult(resourceSpecialCharacteristics);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ResourceSpecialCharacteristicsEditDTO resourceSpecialCharacteristicsDTO)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);

            var ResourceID = _resourceRepo.IsExist(resourceSpecialCharacteristicsDTO.ResourceID);
            if (!ResourceID)
                return CustomResult($"No Resource id is Available With id {resourceSpecialCharacteristicsDTO.ResourceID}", HttpStatusCode.NotFound);
                //return StatusCode((int)HttpStatusCode.NotFound, $"No Resource id is Available With id {resourceSpecialCharacteristicsDTO.ResourceID}");
          
            if(resourceSpecialCharacteristicsDTO.ScheduleID != null)
            {
                var scheduleItemID = _scheduleItemRepo.IsExistWithId(resourceSpecialCharacteristicsDTO?.ScheduleID);
                if (!scheduleItemID)
                    return CustomResult($"No schedule item id is Available With id {resourceSpecialCharacteristicsDTO.ScheduleID}",HttpStatusCode.NotFound);
                    //return StatusCode((int)HttpStatusCode.NotFound, $"No schedule item id is Available With id {resourceSpecialCharacteristicsDTO.ScheduleID}");
            }

            var resourceSpecialCharacteristics = _mapper.Map<ResourceSpecialCharacteristics>(resourceSpecialCharacteristicsDTO);
            var result = await _resourceSpecialCharacteristicsRepository.AddAsync(resourceSpecialCharacteristics);

            return CustomResult(resourceSpecialCharacteristicsDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ResourceSpecialCharacteristicsEditDTO resourceSpecialCharacteristicsDTO)
        {
            if (ModelState.IsValid)
            {
                var resourceExsits = _resourceSpecialCharacteristicsRepository.GetById(id);
                if(resourceExsits != null)
                {
                    ResourceSpecialCharacteristics resourceSpecialCharacteristics = _mapper.Map<ResourceSpecialCharacteristics>(resourceSpecialCharacteristicsDTO);
                    var result = await _resourceSpecialCharacteristicsRepository.EditAsync(id, resourceSpecialCharacteristics,rsc=>rsc.ID);
                    ResourceSpecialCharacteristicsEditDTO resourceSpecialCharacteristicsDTO1  = _mapper.Map<ResourceSpecialCharacteristicsEditDTO>(result);
                    return CustomResult(resourceSpecialCharacteristicsDTO1);
                }
            }
            return CustomResult("All Data Required", HttpStatusCode.BadRequest);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            var result =  _resourceSpecialCharacteristicsRepository.GetById(id);
            if (result == null)
                return CustomResult($"No RSC Found For This Id {id}", HttpStatusCode.NotFound);
            
            await _resourceSpecialCharacteristicsRepository.DeleteSoft(id);
            return CustomResult(HttpStatusCode.NoContent);
        }

    }
}
