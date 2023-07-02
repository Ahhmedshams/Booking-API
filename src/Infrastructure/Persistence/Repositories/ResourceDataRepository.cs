using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Application.Common.Model;

namespace Infrastructure.Persistence.Repositories
{
    public class ResourceDataRepository : CRUDRepository<ResourceData>, IResourceDataRepo
    {
        public ResourceDataRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ResourceData> AddRange(IEnumerable<ResourceData> entities)
        {
            _context.ResourceData.AddRange(entities);
            _context.SaveChanges();
            return entities;
        }
      

        public async Task<IEnumerable<ResourceData>> AddRangeAsync(IEnumerable<ResourceData> entities)
        {
            await _context.ResourceData.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }


        public async Task<IEnumerable<ResourceData>> FindAsync(Expression<Func<ResourceData, bool>> predicate)
        {
            return await base.FindAsync(predicate);
        }


        public async Task<ResourceData> FindAsync(int ResourceId, int AttributeId)
        {
            return await _context.ResourceData.FirstOrDefaultAsync(r => r.AttributeId == AttributeId && r.ResourceId == ResourceId);
        }

        public async Task< AllResourceData> GetAllReourceData(int id )
        {
            var Resource = await _context.Resource.Include(res=> res.ResourceType).Include(res=>res.Images).FirstOrDefaultAsync(res => res.Id == id) ;

            if (Resource == null)
                return null ;

            var Review = await _context.ResourceReview.Where(rr => rr.ResourceId == id)
                .GroupBy(rr => rr.ResourceId)
                .Select(Result => new {
                    AverageRating = Result.Average(rr => rr.Rating),
                    NumRatings = Result.Count()
                }).FirstOrDefaultAsync();



            AllResourceData Result = new() { Id= id ,
                Name= Resource.Name ,
                Price = Resource.Price ,
                ResourceTypeId = Resource.ResourceTypeId ,
                ResourceTypeName = Resource.ResourceType.Name ,
                AverageRating = Review?.AverageRating ?? 5,
                NumRatings = Review?.NumRatings ?? 0,
                Images= Resource.Images
            };


            



            var Attributes = await (from RData in _context.ResourceData
                                    join attr in _context.ResourceMetadata on RData.AttributeId equals attr.AttributeId
                                    where RData.ResourceId == id
                                    select new Application.Common.Model.Attribute
                                    {
                                          Name = attr.AttributeName,
                                          Value = RData.AttributeValue
                                      }).ToListAsync();

            Result.Attributes = Attributes;

            return Result;


        }

        public async Task< List<AllResourceData> > GetAllData()
        {
           var Resources =  await _context.Resource.Include(res => res.ResourceType).ToListAsync();

            if (Resources == null)
                return null;

            var Query = from R in _context.Resource
                         join RData in _context.ResourceData on R.Id equals RData.ResourceId
                         join Mdata in _context.ResourceMetadata on RData.AttributeId equals Mdata.AttributeId
                         select new
                         {
                             Id = R.Id,
                             AttributeName = Mdata.AttributeName,
                             AttributeValue = RData.AttributeValue
                         };


            List< AllResourceData> Result = new List< AllResourceData >();
            foreach (var Resource in Resources)
            {
                var Review = _context.ResourceReview.Where(rr => rr.ResourceId == Resource.Id)
                .GroupBy(rr => rr.ResourceId)
                .Select(Result => new {
                    AverageRating = Result.Average(rr => rr.Rating),
                    NumRatings = Result.Count()
                }).FirstOrDefault();

                AllResourceData Record = new()
                {
                    Id = Resource.Id,
                    Name = Resource.Name,
                    Price = Resource.Price,
                    ResourceTypeId = Resource.ResourceTypeId,
                    ResourceTypeName = Resource.ResourceType.Name,
                    AverageRating = Review?.AverageRating ?? 5,
                    NumRatings = Review?.NumRatings ?? 0
                };

                var attrbutes =  Query.Where(r => r.Id == Record.Id);
                foreach(var attrbute in attrbutes)
                {
                    Application.Common.Model.Attribute attribute = new();

                    attribute.Name = attrbute.AttributeName;
                    attribute.Value = attrbute.AttributeValue;

                    Record.Attributes.Add(attribute);
                }

                Result.Add(Record);
            }

            return Result;


            
        }

        public async Task<List<AllResourceData>> GetAllDataByType(int id)
        {
            var Resources = await _context.Resource.Include(res => res.ResourceType).Where(res=> res.ResourceTypeId == id).ToListAsync();

            if (Resources == null)
                return null;

            var Query = from R in _context.Resource
                        join RData in _context.ResourceData on R.Id equals RData.ResourceId
                        join Mdata in _context.ResourceMetadata on RData.AttributeId equals Mdata.AttributeId
                        where R.ResourceTypeId == id
                        select new
                        {
                            Id = R.Id,
                            AttributeName = Mdata.AttributeName,
                            AttributeValue = RData.AttributeValue
                        };


            List<AllResourceData> Result = new List<AllResourceData>();
            foreach (var Resource in Resources)
            {
                var Review = _context.ResourceReview.Where(rr => rr.ResourceId == Resource.Id)
                .GroupBy(rr => rr.ResourceId)
                .Select(Result => new {
                    AverageRating = Result.Average(rr => rr.Rating),
                    NumRatings = Result.Count()
                }).FirstOrDefault();

                AllResourceData Record = new()
                {
                    Id = Resource.Id,
                    Name = Resource.Name,
                    Price = Resource.Price,
                    ResourceTypeId = Resource.ResourceTypeId,
                    ResourceTypeName = Resource.ResourceType.Name,
                    AverageRating = Review?.AverageRating ?? 5,
                    NumRatings = Review?.NumRatings ?? 0
                };

                var attrbutes = Query.Where(r => r.Id == Record.Id);
                foreach (var attrbute in attrbutes)
                {
                    Application.Common.Model.Attribute attribute = new();

                    attribute.Name = attrbute.AttributeName;
                    attribute.Value = attrbute.AttributeValue;

                    Record.Attributes.Add(attribute);
                }

                Result.Add(Record);
            }

            return Result;



        }

        public async Task<bool> IsExistAsync(Expression<Func<ResourceData, bool>> predicate)
        {
            return await _context.ResourceData.AnyAsync(predicate);
        }
    }


}
