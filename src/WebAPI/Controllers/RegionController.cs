using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : BaseController
    {
       
        private readonly IRegionRepository _regionRepo;
        private readonly IMapper _mapper;



        public RegionController( IRegionRepository regionRepository, IMapper mapper)
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
            var regionDtos = _mapper.Map<IEnumerable<RegionDTO>>(regions);

            if (regionDtos == null || !regionDtos.Any())
            {
                return NotFound("No regions found.");
            }

            return Ok(regionDtos);
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

            var regionDto = _mapper.Map<RegionDTO>(region);
            return Ok(regionDto);
        }

  

        [HttpPost]
        public async Task<IActionResult> Create(RegionDTO regionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = _mapper.Map<Region>(regionDto);
            await _regionRepo.AddAsync(region);

            var createdRegionDto = _mapper.Map<RegionDTO>(region);
            return Ok(createdRegionDto);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedRegion = await _regionRepo.DeleteAsync(id);

            if (deletedRegion == null)
            {
                return NotFound($"Region with ID {id} not found.");
            }

            var regionDto = _mapper.Map<RegionDTO>(deletedRegion);
            return Ok(regionDto);
        }

        //[HttpPut("{id}")]

        //public async Task<IActionResult> Update(int id, RegionDTO regionDto)
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

        //    var region = _mapper.Map<Region>(regionDto);
        //    await _regionRepo.UpdateRegion(region);
        //    return Ok(region);
        //}



        //[HttpPut("{id}")]
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
