using AutoMapper;
namespace WebAPI.Profiles
{
    public class RegionMapper : Profile
    {
        public RegionMapper()
        {
           
            CreateMap<RegionDTO, Region>().ReverseMap();
            CreateMap<Region, RegionDTO>().ReverseMap();

        }
    }
}
