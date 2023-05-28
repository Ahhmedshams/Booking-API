using AutoMapper;
using Infrastructure.Identity;

namespace Domain.Common
{
    public class AuthAutoMapper : Profile
    {
        public AuthAutoMapper()
        {
            CreateMap<RegisterUserDto, ApplicationUser>().ReverseMap();
        }
    }
}
