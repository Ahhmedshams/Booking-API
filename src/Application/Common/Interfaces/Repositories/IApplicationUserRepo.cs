using Domain.Identity;

namespace Application.Common.Interfaces.Repositories
{
    public interface IApplicationUserRepo
    {
        Task<IEnumerable<ApplicationUser>> UserReport(DateTime startDate , DateTime endDate);
    }
}
