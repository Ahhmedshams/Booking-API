using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Profiles
{
    public class RoleMapper: Profile
    {
        public RoleMapper() { 
        
            CreateMap<RoleDTO, IdentityRole>().ReverseMap();
        }
    }
}
