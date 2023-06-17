using Domain.Enums;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeds
{
    public static class UsersSeeders
    {

        public static async Task SeedNormalUsersAsync(UserManager<ApplicationUser> userManager)
        {

            var appUser = new ApplicationUser()
            {
                UserName="Amar_Ghandour",
                FirstName="Amar",
                LastName= "Ghandour",
                Email="amarghandour89@gmail.com",
                EmailConfirmed=true
            };


            var user = await userManager.FindByEmailAsync(appUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(appUser, "123@Server");
                await userManager.AddToRoleAsync(appUser, Roles.User.ToString());
            }

        }

        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var appUser = new ApplicationUser()
            {
                UserName="Amr_Allam",
                FirstName="Amr",
                LastName= "Allam",
                Email="amarelshamlyelghandour@gmail.com",
                EmailConfirmed=true
            };


            var user = await userManager.FindByEmailAsync(appUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(appUser, "123@Server");
                await userManager.AddToRolesAsync(appUser, new List<string>() { Roles.User.ToString() , Roles.Admin.ToString() } );
            }

            await SeedClaimForAdminAsync(roleManager);
    
        }

        private static async Task SeedClaimForAdminAsync(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin.ToString());

            if (adminRole != null)
            {
               await roleManager.AddPermissionClaimAsync(adminRole, Entities.ResourceTypes.ToString());
            }
        }
    }
}
