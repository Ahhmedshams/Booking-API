using AutoMapper;
using Domain.Identity;

namespace WebAPI.Profiles
{
    public class ApplicationUserMapper : Profile
    {
        public ApplicationUserMapper()
        {
            CreateMap<ApplicationUser, ApplicationUserDTO>()
            //    .ForMember(des => des.Roles , opt => opt.MapFrom(src => src.))
                .ReverseMap();


        }
    }
}
