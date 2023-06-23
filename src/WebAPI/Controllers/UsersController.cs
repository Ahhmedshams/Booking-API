using AutoMapper;
using CoreApiResponse;
using Domain.Identity;
using Infrastructure.Persistence;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "GuestUser")]

    public class UsersController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IMapper mapper)
        {
            this.userManager=userManager;
            this.roleManager=roleManager;
            this.context=context;
            this.mapper=mapper;
        }



        [HttpGet]
       // [Authorize(Policy = "GuestUser")]

       //[Authorize(Permissions.Users.Index)]

        public async Task<IActionResult> GetAll()
        {
            var users = await userManager.Users.ToListAsync();


            List<ApplicationUserDTO> userDTOs = mapper.Map<List<ApplicationUserDTO>>(users);

            foreach(var userDto in  userDTOs)
            {
                var user = await userManager.FindByEmailAsync(userDto.Email);

                userDto.Roles = (List<string>) userManager.GetRolesAsync(user).Result;
            }

            return CustomResult(userDTOs);
        }


        [HttpGet("ManageRoles/{id:Guid}")]
        public async Task<IActionResult> ManageRoles(string id)
        {

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
                return CustomResult("User Not Found", System.Net.HttpStatusCode.NotFound);

            var allRoles = await roleManager.Roles.ToListAsync();

            var res = new
            {
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                Roles = allRoles.Select(r => new
                {
                    RoleName = r.Name,
                    IsSelected = userManager.IsInRoleAsync(user, r.Name).Result
                })
            };

            return CustomResult(res);
        }

        [HttpPost("AssignRoles")]
        public async Task<IActionResult> UpdateRoles(UserRolesDTO userRolesDTO)
        {
            var user = await userManager.FindByIdAsync(userRolesDTO.UserID);

            if (user == null)
                return CustomResult("User Not Found", System.Net.HttpStatusCode.NotFound);

            var userRoles = await userManager.GetRolesAsync(user);

            if (userRoles.Any())
                await userManager.RemoveFromRolesAsync(user, userRoles);
           
            await userManager.AddToRolesAsync(user, userRolesDTO.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));


            return CustomResult("Roles assign Successfully", System.Net.HttpStatusCode.Created);
        }


    }
}
