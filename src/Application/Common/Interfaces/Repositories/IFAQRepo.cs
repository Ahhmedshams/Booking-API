using Domain.Entities;

namespace Application.Common.Interfaces.Repositories
{
    public interface IFAQRepo: IAsyncRepository<FAQ>
    {
        public Task<FAQ> FindByQuestion(string question);
        public Task<bool> CategoryExits(int categoryId);
    }
}
