using AutoMapper;
using WebAPI.DTO;

namespace WebAPI.Profiles
{
    public class FAQMapper:Profile
    {
        public FAQMapper()
        {
            CreateMap<FAQ, FAQAddDTO>().ReverseMap();
            CreateMap<FAQ, FAQGetDTO>().ReverseMap();
        }
    }
}
