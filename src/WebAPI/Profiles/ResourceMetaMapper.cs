using AutoMapper;

namespace WebAPI.profiles
{
    public class ResourceMetaMapper : Profile
    {
        public ResourceMetaMapper() 
        {
            CreateMap<ResourceMetadata, ResourceMetaReqDTO>().ReverseMap();
            CreateMap<ResourceMetadata, ResourceMetaRespDTO>().ReverseMap();
        }
    }

    

}
