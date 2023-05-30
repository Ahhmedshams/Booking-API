using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleItemController : BaseController
    {
        /*[HttpGet]
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

        }*/

    }
}
