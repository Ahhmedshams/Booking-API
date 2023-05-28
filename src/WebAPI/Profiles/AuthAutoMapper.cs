using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Identity;
using WebAPI.DTO;

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
