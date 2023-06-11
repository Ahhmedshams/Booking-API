using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IScheduleRepo scheduleRepo;

        public ScheduleController(IMapper mapper, IScheduleRepo scheduleRepo)
        {
            this.mapper = mapper;
            this.scheduleRepo = scheduleRepo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Schedule> resource = scheduleRepo.GetAll();
            if (resource.Count() == 0)
                return CustomResult("No Schedule Are Available", HttpStatusCode.NotFound);

            return CustomResult(resource);
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

        [HttpGet("GetAvailableResources")]
        public IActionResult GetAvailableResources(DateTime fromDate, DateTime toDate, int resourceTypeId)
        {
            var availableResources = scheduleRepo.GetAvailableResources(fromDate, toDate, resourceTypeId);

            return Ok(availableResources);
        }

        [HttpGet("{resourceId:int}")]
        public IActionResult GetByResourceId(int resourceId)
        {
            Schedule schedule = scheduleRepo.GetByResourceId(resourceId);
            if (schedule == null)
                return CustomResult($"No Schedule Founded With Resource ID{resourceId}", HttpStatusCode.NotFound);
            var result = mapper.Map<ScheduleDTO>(schedule);
            return Ok(result);
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
            return BadRequest("All Data Required");

        }

        [HttpDelete("SoftDelete/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
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
