using Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceReviewRepository : CRUDRepository<ResourceReview>, IResourceReviewRepo
    {
        public ResourceReviewRepository( ApplicationDbContext context ) : base( context ) { }

    }
}
