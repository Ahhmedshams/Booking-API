using Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class RoleManagerExtension
    {

        public static async Task AddPermissionClaimAsync(this RoleManager<IdentityRole> roleManager, IdentityRole role, string model)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var permissions = Permissions.GeneratePermissionsList(model);

            foreach(var permission in permissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                     await roleManager.AddClaimAsync(role, new Claim("Permission", permission));

            }
        }
    }
}
