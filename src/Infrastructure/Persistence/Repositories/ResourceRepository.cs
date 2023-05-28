using Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceRepository : CRUDRepository<Resource>, IResourceRepo
    {
        public ResourceRepository(ApplicationDbContext context) : base(context)
        {
        }


        public bool IsExist(int id)
        {
            return _context.Resource.Any(res => res.Id == id);
        }

        public Resource EditPrice(int id, decimal price)
        {
            var foundEntity = _context.Resource.Find(id);
            if (foundEntity == null) return null;
            foundEntity.Price = price;
               _context.SaveChanges();
            return foundEntity;
        }
    }
}
