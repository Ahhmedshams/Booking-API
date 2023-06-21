    using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class FAQRepository : CRUDRepository<FAQ>, IFAQRepo
    {
        public FAQRepository(ApplicationDbContext context) : base(context)
        {}
        public async Task<FAQ>FindByQuestion(string question)
        {
             return await _context.FAQ.FirstOrDefaultAsync(x => x.Question == question.Trim());
        }
    }
}
