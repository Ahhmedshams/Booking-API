using Application.Common.Helpers;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Specification
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification()
        {
        }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria.Add(criteria);
        }

        public List<Expression<Func<T, bool>>> Criteria { get; private set; } = new List<Expression<Func<T, bool>>>();

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>> ();

        public Expression<Func<T, object>> OrderBy { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnable { get; private set; }

        protected void AddIncludes(Expression<Func<T, object>> include)
        {
            Includes.Add(include);
        }

        protected void AddOrderBy (Expression<Func<T, object>> orderBy)
        {
            OrderBy = orderBy;
        }

        protected void AddSearchBy (Expression<Func<T, bool>> search)
        {
            Criteria.Add(search);
        }
        protected void ApplyPagging(int skip , int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnable = true;
        }
    }
}
