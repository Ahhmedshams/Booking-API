using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : BaseController
    {

        private readonly IRegionRepository _regionRepo;
        private readonly IMapper _mapper;



        public RegionController(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepo = regionRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var regions = await _regionRepo.GetAllAsync();
            var regionDtos = _mapper.Map<IEnumerable<RegionGetDTO>>(regions);

            if (regionDtos == null || !regionDtos.Any())
            {
                return NotFound("No regions found.");
            }

            return CustomResult(regionDtos);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = await _regionRepo.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound($"Region with ID {id} not found.");
            }

            var regionDto = _mapper.Map<RegionGetDTO>(region);
            return CustomResult(regionDto);
        }



        [HttpPost]
        public async Task<IActionResult> Create(RegionAddDTO regionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = _mapper.Map<Region>(regionDto);
            await _regionRepo.AddAsync(region);

            var createdRegionDto = _mapper.Map<RegionGetDTO>(region);
            return CustomResult(createdRegionDto);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedRegion = await _regionRepo.DeleteAsync(id);

            if (deletedRegion == null)
            {
                return NotFound($"Region with ID {id} not found.");
            }

            var regionDto = _mapper.Map<RegionGetDTO>(deletedRegion);
            return CustomResult(regionDto);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, RegionAddDTO regionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var regionToUpdate = await _regionRepo.GetByIdAsync(id);

            if (regionToUpdate == null)
            {
                return NotFound($"Region with ID {id} not found.");
            }

            var region = _mapper.Map<Region>(regionDto);

            await _regionRepo.EditAsync( id , region, r => r.Id);
            return CustomResult(region);
        }



        // [HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, Region region)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var regionToUpdate = await _regionRepo.GetByIdAsync(id);

        //    if (regionToUpdate == null)
        //    {
        //        return NotFound($"Region with ID {id} not found.");
        //    }

        //    await _regionRepo.EditAsync(region);
        //    return Ok(region);
        //}

    }
}
