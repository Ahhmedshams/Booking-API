﻿using Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IResourceReviewRepo: IAsyncRepository<ResourceReview> ,IRepository<ResourceReview> 
    {
        Task<ResourceReview> Patch(int id ,ResourceReview resourceReview);
        public  Task SetRating(int id);

    }
}
