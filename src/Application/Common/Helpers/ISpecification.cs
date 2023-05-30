using System.Linq.Expressions;

namespace Application.Common.Helpers
{
    public interface ISpecification<T>
    {
        List<Expression<Func<T, bool>>> Criteria { get;}
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }

        int Take { get;}
        int Skip { get; }
        bool IsPagingEnable { get;}
    }
}
