using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ResourceReview> Patch(int id, ResourceReview resourceReview)
        {
            var FoundReview =  _context.ResourceReview.FirstOrDefault( r => r.Id == id );
            if (FoundReview == null)
                return null;
            
            FoundReview.Rating = resourceReview?.Rating ?? FoundReview.Rating;
            FoundReview.Description = resourceReview?.Description ?? FoundReview.Description;

            await _context.SaveChangesAsync();
            return FoundReview;

        }

        public async Task SetRating(int id)
        {
            var resourceID = id;
            var sql = $"EXEC SetRating @resourceID";
            var parameter = new SqlParameter("@resourceID", resourceID);

            await _context.Database.ExecuteSqlRawAsync(sql, parameter);


            await _context.SaveChangesAsync();
        }

    }
}
