﻿using Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceTypeRepository : CRUDRepository<ResourceType>, IResourceTypeRepo
    {
        public ResourceTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public bool IsExist(int id)
        {
            return _context.ResourceTypes.Any(res => res.Id == id);
        }
    }
}