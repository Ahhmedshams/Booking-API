using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> IsExistAsync(int id)
        {
            return await _context.ResourceTypes.AnyAsync(res => res.Id == id);

        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await _context.ResourceTypes.AnyAsync(res => res.Name == name);

        }


        public  async Task<bool> SoftDeleteAsync(int id)
        {
            var ResourceType = _context.ResourceTypes.Find(id);
            if (ResourceType == null)
                return false;

            ResourceType.IsDeleted = true;

            DeleteResourceMetaData(id);

            DeleteResources(id);

            await _context.SaveChangesAsync();
            return true;    
        }




        private void DeleteResourceMetaData(int id)
        {
            _context.ResourceMetadata.Where(res => res.ResourceTypeId == id).ToList().ForEach(res =>
                    res.IsDeleted = true
                );
        }

        private void DeleteResources(int id)
        {
            

            var Resources = _context.Resource.Where(res => res.ResourceTypeId == id).ToList();

            Resources.ForEach(res =>
                res.IsDeleted = true
            );

            foreach (var resource in Resources)
            {
                _context.ResourceData.Where(res => res.ResourceId == resource.Id).ToList().ForEach(res =>
                res.IsDeleted = true
                );

                 DeleteResourceSchedule(resource.Id);
            }

        }


        private void DeleteResourceSchedule(int id)
        {
            var Schedules = _context.Schedule.Where(sch => sch.ResourceId == id).ToList();


            foreach (var Schedule in Schedules)
            {
                Schedule.IsDeleted = true;
                _context.ScheduleItem.Where(sch => sch.ScheduleId == Schedule.ScheduleID).ToList().ForEach(schItem =>
                        schItem.IsDeleted = false
                );
            }
        }

    }
}
