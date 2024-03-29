﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{

    public class RegionRepository : CRUDRepository<Region>, IRegionRepository
    {
        public RegionRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task SoftDeleting(int id)
        {
            var region = await GetByIdAsync(id);
            region.IsDeleted = true;
            await _context.SaveChangesAsync();
        }





    }








}

