using Domain.Identity;

namespace Application.Common.Interfaces.Repositories
{
    public interface IApplicationUserRepo
    {
        Task<int> UserReport(DateTime startDate , DateTime endDate);
    }
}
