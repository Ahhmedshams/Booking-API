using AutoMapper;

namespace WebAPI.Profiles
{
    public class FAQCategoryMapper : Profile
    {
        public FAQCategoryMapper()
        {
            CreateMap<FAQCategoryAddDTO, FAQCategory>();
        }
    }
}