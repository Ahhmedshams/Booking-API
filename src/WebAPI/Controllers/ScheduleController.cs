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

        public ScheduleController(IMapper mapper,IScheduleRepo scheduleRepo) 
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
        public IActionResult Add(Schedule schedule)
        {

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            var res = scheduleRepo.Add(schedule);
            return CustomResult(res);

        }


    }
}
