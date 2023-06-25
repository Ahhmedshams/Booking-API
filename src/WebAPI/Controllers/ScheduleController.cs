using Application.Common.Interfaces.Repositories;
using Application.Common.Model;
using Application.Common.Models;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System.Net;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IScheduleRepo scheduleRepo;

        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;

        public ScheduleController(IMapper mapper, IScheduleRepo scheduleRepo, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions)
        {
            this.mapper = mapper;
            this.scheduleRepo = scheduleRepo;
            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value; 
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] SieveModel sieveModel)
        {
            IEnumerable<Schedule> resource = scheduleRepo.GetAll();
            //if (resource.Count() == 0)
            //    return CustomResult("No Schedule Are Available", HttpStatusCode.NotFound);

            var FilteredSchedules = _sieveProcessor.Apply(sieveModel, resource.AsQueryable());

            return CustomResult(FilteredSchedules);
        }

        [HttpPost("Add")]
        public IActionResult Add(ScheduleReqDTO schedule)
        {
            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            var reqSchedule = mapper.Map<Schedule>(schedule);

            var res = scheduleRepo.AddSchedule(reqSchedule);
            return CustomResult(res);
        }

        [HttpGet("GetSchedules")]
        public IActionResult GetSchedulesByDateRange(DateTime fromDate, DateTime toDate, [FromQuery] SieveModel sieveModel)
        {
            var schedules = scheduleRepo.GetSchedules(fromDate, toDate, sieveModel);

            var FilteredSchedules = _sieveProcessor.Apply<AvailableSchedule>(sieveModel, schedules.AsQueryable());

            return CustomResult(FilteredSchedules);
        }

        [HttpGet("GetAvailableResources")]
        public IActionResult GetAvailableResources([FromQuery] string _day, [FromQuery] int _serviceId, [FromQuery] string _startTime, [FromQuery] string _endTime, [FromQuery] SieveModel sieveModel, [FromQuery] int? RegionId)
        {
            var availableResources = scheduleRepo.GetAvailableResources(_day, _serviceId, _startTime, _endTime, sieveModel, RegionId);

            if (availableResources.Count == 0)
            {
                return CustomResult(new List<Resource>());
            }
            else
            {
                //var responseAvailableResources = availableResources.Select(r => new AllResourceData
                //{
                //    ResourceTypeId = r.ResourceTypeId,
                //    Price = r.Price,
                //    Name = r.Name,
                //    Images = r.Images,
                //}).ToList();
                //var responseAvailableResources = mapper.Map<List<AllResourceData>>(availableResources);
                IQueryable<Resource>? FilteredAvailableResources = _sieveProcessor.Apply<Resource>(sieveModel, availableResources.AsQueryable());

                var availableRess = mapper.Map<List<ResourceRespDTO>>(FilteredAvailableResources);


                return CustomResult(availableRess);
            }
        }

        [HttpGet("{resourceId:int}")]
        public IActionResult GetByResourceId(int resourceId)
        {
            Schedule schedule = scheduleRepo.GetByResourceId(resourceId);
            //if (schedule == null)
            //    return CustomResult($"No Schedule Founded With Resource ID{resourceId}", HttpStatusCode.NotFound);
            var result = mapper.Map<ScheduleDTO>(schedule);
            return CustomResult(result);
        }

        [HttpPut("Edit/{scheduleId:int}")]
        public async Task<IActionResult> Edit(int scheduleId,ScheduleReqDTO scheduleReqDTO)
        {
            if (ModelState.IsValid)
            {
                bool scheduleCheck = scheduleRepo.IsExist(scheduleId);
                if (scheduleCheck)
                    {
                        Schedule schedule  =mapper.Map<Schedule>(scheduleReqDTO);
                        var result = await scheduleRepo.EditAsync(scheduleId, schedule, res => res.ScheduleID);
                        ScheduleDTO scheduleDto = mapper.Map<ScheduleDTO>(result);
                        return CustomResult(result);
                    }

            }
            return CustomResult("All Data Required", HttpStatusCode.BadRequest);

        }

        [HttpDelete("SoftDelete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
                return CustomResult($"No Schedule Founded With id {id}", HttpStatusCode.NotFound);

            var result = await scheduleRepo.ScheduleSoftDelete(id);
            if (result == null)
                return CustomResult($"No Schedule Founded With id {id}", HttpStatusCode.NotFound);

            return CustomResult(result, HttpStatusCode.OK);
        }

    }
}
