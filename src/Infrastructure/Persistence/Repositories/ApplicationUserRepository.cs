using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IEnumerable<ApplicationUser>> UserReport(DateTime startDate, DateTime endDate)
        {
            var users = await _userManager.Users
                            .Where(u => u.CreatedOn>= startDate && u.CreatedOn<= endDate)
                            .ToListAsync();
            return users;
        }
    }
}
