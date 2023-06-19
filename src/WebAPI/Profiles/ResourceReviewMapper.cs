using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceReviewMapper : Profile
    {
        public ResourceReviewMapper() 
        {
            CreateMap<ResourceReview, ResourceReviewDTO>().ReverseMap();
            CreateMap<ResourceReview, ResourceReviewResDTO>().ReverseMap();
           // CreateMap< List<ResourceReview>, List< ResourceReviewResDTO>>().ReverseMap();


        }
    }
}
