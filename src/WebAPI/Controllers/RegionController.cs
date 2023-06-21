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
       

        public RegionController( IRegionRepository regionRepository )
        {
            _regionRepo = regionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regions = await _regionRepo.GetAllAsync();
            return Ok(regions);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var region = await _regionRepo.GetByIdAsync(id);
            return Ok(region);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Region region)
        {
            await _regionRepo.AddAsync(region);
            return Ok(region);
        }
        //[HttpPut]
        //public async Task<IActionResult> Update(Region region)
        //{
        //    await _regionRepo.UpdateAsync(region);
        //    return Ok(region);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var region = await _regionRepo.GetByIdAsync(id);
            await _regionRepo.DeleteAsync(region);
            return Ok(region);
        }

    }
}
