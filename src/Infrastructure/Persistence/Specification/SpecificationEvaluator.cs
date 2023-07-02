using Application.Common.Helpers;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Specification
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;
			query = query.Where(x => x.IsDeleted == false);

			//Filter
			if (spec.Criteria.Count() != 0)
            {
                foreach(var criteria in spec.Criteria)
                {
                    query = query.Where(criteria);
                }
            }

            //Sort
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            //Paging
            if (spec.IsPagingEnable)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
