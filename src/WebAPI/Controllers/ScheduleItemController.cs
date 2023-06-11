using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleItemController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IScheduleItemRepo scheduleItemRepo;

        public ScheduleItemController(IMapper mapper, IScheduleItemRepo scheduleItemRepo)
        {
            this.mapper = mapper;
            this.scheduleItemRepo = scheduleItemRepo;
        }

        //addrange (to add array shedule item)
        [HttpPost("AddRange/{scheduleId:int}")]
        public async Task<IActionResult> AddRange(int scheduleId, params ScheduleItemDTO[] scheduleItemDTO)
        {
            var IdCheck = CheckScheduleId(scheduleId, scheduleItemDTO);
            if (IdCheck != null)
                return IdCheck;
            /*if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);*/

            var scheduleItems = mapper.Map<IEnumerable<ScheduleItem>>(scheduleItemDTO);
            await scheduleItemRepo.AddRangeAsync(scheduleItems);

            return CustomResult("Data Added Successfully", HttpStatusCode.OK);
        }

        //add (sp add from excel  validatae csv extension)






        [HttpGet("Get All")]
        public IActionResult GetAll()
        {
            IEnumerable<ScheduleItem> items = scheduleItemRepo.GetAll();
            if (items.Count() == 0)
                return CustomResult("No Schedule Are Available", HttpStatusCode.NotFound);
            var result = mapper.Map<IEnumerable<ScheduleItemDTO>>(items);
            return CustomResult(result);
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody]ScheduleItemDTO scheduleItemDto)
        {
           /* Console.WriteLine($"scheduleItemDto: {JsonConvert.SerializeObject(scheduleItemDto)}");*/
            var SchedulCheck = CheckSchedule(scheduleItemDto.ScheduleId);
            if (SchedulCheck != null)
                return SchedulCheck;

            if (!ModelState.IsValid)
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            var scheduleItem = mapper.Map<ScheduleItem>(scheduleItemDto);
            var res = scheduleItemRepo.Add(scheduleItem);
            return CustomResult(res);

        }

        //delete (with the 4 pk values searach by them the delete it (make dto without avaliable column)with condition that
        // available is true )

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id, SmallScheduleItemDTO scheduleItemDTO)
        {
            ScheduleItem item = scheduleItemRepo.Delete(id, scheduleItemDTO.Day, scheduleItemDTO.StartTime, scheduleItemDTO.EndTime);
            if (item == null)
                return CustomResult("Schedule Item Not Found", HttpStatusCode.NotFound);

            return CustomResult(item);

        }

        //update (in case reserved updata available with false)
        [HttpPut("/Edit")]
        public async Task<IActionResult> Edit([FromBody] ScheduleItemDTO scheduleItemDto)
        {
            var data = await scheduleItemRepo.FindAsync(scheduleItemDto.ScheduleId,scheduleItemDto.Day, scheduleItemDto.StartTime, scheduleItemDto.EndTime);
            if (data == null)
                return CustomResult($"No Schedule Item Found With Given Data", HttpStatusCode.NotFound);
            /* var  scheduleCheck = CheckSchedule(sheduleId);
             if (scheduleCheck != null)
                 return CustomResult($"No Schedule Item Found With id {sheduleId}", HttpStatusCode.NotFound);*/

            data.Available = scheduleItemDto.Available;
            await scheduleItemRepo.SaveChangesAsync();

            ScheduleItemDTO scheduleItem = mapper.Map<ScheduleItemDTO>(data);

            /* var result = await scheduleItemRepo.EditAsync(new {ScheduleId =scheduleItem.ScheduleId, Day=scheduleItem.Day,
                 StartTime=scheduleItem.StartTime,EndTime=scheduleItem.EndTime},scheduleItem, s => s.ScheduleId);*/

            /*            var schedItemDTO = mapper.Map<ScheduleItemDTO>(result);
            */
            return CustomResult(scheduleItem);
        }

        private IActionResult CheckScheduleId(int sheduleId, ScheduleItemDTO[] scheduleItemDTO)
        {

            var SchedulCheck = CheckSchedule(sheduleId);
            if (SchedulCheck != null)
                return SchedulCheck;

            var CheckId = scheduleItemDTO.Where(s => s.ScheduleId != sheduleId);

            if (CheckId.Count() != 0)
                return CustomResult($"Not All ScheduleItems Have The Same Schedule ID ", HttpStatusCode.BadRequest);

            return null;
        }

        private IActionResult CheckSchedule(int schedId)
        {
            bool scheduleFound = scheduleItemRepo.IsExistWithId(schedId);
            if (!scheduleFound)
                return CustomResult($"No Schedule Found With id {schedId}", HttpStatusCode.NotFound);
            else
                return null;
        }


    }
}
