using Domain.Entities;

namespace Application.Common.Interfaces.Repositories
{
    public interface IFAQCategoryRepo : IAsyncRepository<FAQCategory>
    {
        public Task<FAQCategory> FindByName(string question);
        public Task<FAQCategory> GetCategoryByIdWithFAQ(int id);

    }
}
