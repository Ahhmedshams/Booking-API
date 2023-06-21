using AutoMapper;

namespace WebAPI.Profiles
{
    public class FAQMapper:Profile
    {
        public FAQMapper()
        {
            CreateMap<FAQ, FAQDTO>().ReverseMap();
        }
    }
}
