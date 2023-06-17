using AutoMapper;
using Domain.Identity;

namespace Domain.Common
{
    public class AuthAutoMapper : Profile
    {
        public AuthAutoMapper()
        {
            CreateMap<RegisterUserDto, ApplicationUser>().ReverseMap();
            CreateMap<UserResponce, ApplicationUser>().ReverseMap(); 
                CreateMap<EditUserDTO, ApplicationUser>().ReverseMap();
        }
    }
}
