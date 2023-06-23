using AutoMapper;
namespace WebAPI.Profiles
{
    public class RegionMapper : Profile
    {
        public RegionMapper()
        {
           
            CreateMap<RegionAddDTO, Region>().ReverseMap();
            CreateMap<Region, RegionGetDTO>().ReverseMap();

        }
    }
}
