using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Filters
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IConfiguration configuration;

        public PermissionAuthorizationHandler(IConfiguration configuration)
        {
            this.configuration=configuration;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }
            var permissionss = context.User.Claims.Where(x => x.Type == "Permission" &&
                                                                x.Value == requirement.Permission &&
                                                                x.Issuer == configuration["JWT:Issuer"]);
            
            if (permissionss.Any())
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
