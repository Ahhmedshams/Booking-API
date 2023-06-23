    using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class FAQRepository : CRUDRepository<FAQ>, IFAQRepo
    {
        public FAQRepository(ApplicationDbContext context) : base(context)
        {}
        public async Task<FAQ>FindByQuestion(string question)
         => await _context.FAQ.FirstOrDefaultAsync(x => x.Question == question.Trim());
        
        public async Task<bool> CategoryExits(int categoryId)
        => await _context.Set<FAQCategory>().AnyAsync(e=>e.Id==categoryId);
        
        
        
    }
}
