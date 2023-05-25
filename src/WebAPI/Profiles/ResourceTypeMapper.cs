using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.profiles
{
    public class ResourceTypeMapper : Profile
    {
        public ResourceTypeMapper() 
        {
            CreateMap<ResourceType, ResourceTypeDTO>().ReverseMap();
        }
    }
}
