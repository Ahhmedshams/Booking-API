    using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class FAQCategoryRepository : CRUDRepository<FAQCategory>, IFAQCategoryRepo
    {
        public FAQCategoryRepository(ApplicationDbContext context) : base(context)
        {}

        public async Task<FAQCategory>FindByName(string name)   
        =>await _context.FAQCategory.FirstOrDefaultAsync(e=>e.Name==name);

        public Task<FAQCategory> GetCategoryByIdWithFAQ(int id)
         =>  _context.FAQCategory.Include(e => e.FAQS).FirstOrDefaultAsync(e => e.Id == id);

    }
}
