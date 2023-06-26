using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using WebAPI.Utility;
using Application.Common.Interfaces.Repositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleItemController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IScheduleItemRepo scheduleItemRepo;
        private readonly IScheduleRepo scheduleRepo;

        public ScheduleItemController(IMapper mapper, IScheduleItemRepo scheduleItemRepo,IScheduleRepo scheduleRepo)
        {
            this.mapper = mapper;
            this.scheduleItemRepo = scheduleItemRepo;
            this.scheduleRepo = scheduleRepo;
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
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<ScheduleItem> items =await scheduleItemRepo.GetAllScheduleItems();
            
            return CustomResult(items.ToScheduleItem());
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
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit(EditScheduleItemDTO scheduleItemDto)
        {
            if (ModelState.IsValid)
            {
                var data = await scheduleItemRepo.FindAsync(scheduleItemDto.ScheduleId, scheduleItemDto.oldDay, scheduleItemDto.oldStartTime, scheduleItemDto.oldEndTime);
                if (data == null)
                    return CustomResult($"No Schedule Item Found With Given Data", HttpStatusCode.NotFound);

                //var scheduleCheck = CheckSchedule(scheduleItemDto.ScheduleId);
                //if (scheduleCheck != null)
                //    return CustomResult($"No Schedule Item Found With id {scheduleItemDto.ScheduleId}", HttpStatusCode.NotFound);

                var schedule = scheduleRepo.GetById(scheduleItemDto.ScheduleId);
                if (schedule == null)
                    return CustomResult($"No Schedule Found With id {scheduleItemDto.ScheduleId}", HttpStatusCode.NotFound);
                if (scheduleItemDto.newDay < schedule.FromDate || scheduleItemDto.newDay > schedule.ToDate)
                    return CustomResult($"The day should be in the range of the schedule from {schedule.FromDate} to {schedule.ToDate}", HttpStatusCode.BadRequest);

                // Check if the start time and end time are not already existed in the schedule item in the same day
                var existingScheduleItems = await scheduleItemRepo.FindByDayAsync(scheduleItemDto.ScheduleId, scheduleItemDto.newDay);
                if (existingScheduleItems != null)
                {
                    foreach (var existingScheduleItem in existingScheduleItems)
                    {
                        if (existingScheduleItem.ID == data.ID)
                            continue;

                        if ((scheduleItemDto.newStartTime >= existingScheduleItem.StartTime && scheduleItemDto.newStartTime < existingScheduleItem.EndTime) ||
                            (scheduleItemDto.newEndTime > existingScheduleItem.StartTime && scheduleItemDto.newEndTime <= existingScheduleItem.EndTime))
                            return CustomResult($"The start time and end time should not overlap with the existing schedule item for the same day", HttpStatusCode.BadRequest);

                    }
                }
                // Check if the available change is for a future day
                //if (scheduleItemDto.Available && scheduleItemDto.newDay < DateTime.Now.Date)
                //    return CustomResult($"The status can only be changed to Available for future days", HttpStatusCode.BadRequest);

                // Update the schedule item
                data.Day = scheduleItemDto.newDay;
                data.StartTime = scheduleItemDto.newStartTime;
                data.EndTime = scheduleItemDto.newEndTime;
                data.Available = scheduleItemDto.Available;
                data.Shift = scheduleItemDto.Shift;

                await scheduleItemRepo.SaveChangesAsync();

                ScheduleItemDTO updatedScheduleItemDto = mapper.Map<ScheduleItemDTO>(data);

                return CustomResult(updatedScheduleItemDto);
            }

                /* var result = await scheduleItemRepo.EditAsync(new {ScheduleId =scheduleItem.ScheduleId, Day=scheduleItem.Day,
                     StartTime=scheduleItem.StartTime,EndTime=scheduleItem.EndTime},scheduleItem, s => s.ScheduleId);*/

                /*            var schedItemDTO = mapper.Map<ScheduleItemDTO>(result);
                */
            return CustomResult(ModelState, HttpStatusCode.BadRequest);
            
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
