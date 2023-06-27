using AutoMapper;

namespace WebAPI.Profiles
{
    public class ResourceMapper: Profile
    {
        public ResourceMapper()
        {
            //CreateMap<Resource, ResourceRespDTO>();

			CreateMap<Resource, ResourceRespDTO>()
			    .ForMember(dest => dest.ImageUrls,
						    opt => opt.MapFrom(src =>
								    src.Images.Select(s => s.Uri).ToList()));

			CreateMap<ResourceReqDTO,Resource>().ReverseMap(); 
            CreateMap<Resource, ResourceWithDataDTO>().ReverseMap(); 
        }
    }
}
