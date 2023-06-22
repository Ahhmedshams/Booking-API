using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextInitializer
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ApplicationDbContextInitializer(ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            this.applicationDbContext=applicationDbContext;
            this.userManager=userManager;
            this.roleManager=roleManager;
        }

        public async Task InitailizeAsync()
        {
            try
            {
                if (applicationDbContext.Database.IsSqlServer())
                    await applicationDbContext.Database.MigrateAsync();
            }
            catch(Exception ex)
            {
                // TODO: add logger and exception
            }
          
        }

        public async Task SeedAsync()
        {
            try
            {
                await Seeds.RolesSeeder.SeedRolesAsync(roleManager);
                await Seeds.UsersSeeders.SeedNormalUsersAsync(userManager);
                await Seeds.UsersSeeders.SeedAdminUserAsync(userManager, roleManager);

                //await Seeds.ReourcesTypesSeeder.SeedResoucesTypesSchudaulShownAsync(applicationDbContext);
                // await Seeds.ReourcesSeeder.SeedResourcesAsync(applicationDbContext);


                await applicationDbContext.SaveChangesAsync();

            }catch(Exception ex)
            {
                // TODO: add logger and exception

            }
        }
    }
}
