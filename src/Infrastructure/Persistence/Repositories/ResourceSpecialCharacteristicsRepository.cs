using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceSpecialCharacteristicsRepository:CRUDRepository<ResourceSpecialCharacteristics>,IResourceSpecialCharacteristicsRepo
    {
        public ResourceSpecialCharacteristicsRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _context.Resource.AnyAsync(res => res.Id == id);
        }
        public async Task DeleteSoft(int id)
        {
            var RSC = await GetByIdAsync(id);

            if (RSC != null)
            {
                _context.ResourceSpecialCharacteristics.Remove(RSC);
                await _context.SaveChangesAsync();
            }
        }

    }
}
