using AutoMapper;
using CoreApiResponse;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "GuestUser")]

    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public RolesController(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            this.roleManager=roleManager;
            this.mapper=mapper;
        }


        [HttpGet]
       //[Authorize(Policy = "GuestUser")]

        public async Task<IActionResult> GetAll()
        {
            var roles = await roleManager.Roles.ToListAsync();

            List<RoleDTO> roleDTOs = mapper.Map<List<RoleDTO>>(roles);

            return CustomResult(roleDTOs);
        }

        [HttpPost("CreateRole")]
        
        public async Task<IActionResult> Create(RoleDTO roleDTO)
        {

            if (!ModelState.IsValid)
                return  CustomResult(ModelState, HttpStatusCode.BadRequest);

            var roleExist = await roleManager.FindByNameAsync(roleDTO.Name);

            if (roleExist != null)
                return CustomResult("Role is Already Exist.",HttpStatusCode.BadRequest);

           var result = await roleManager.CreateAsync(new IdentityRole() { Name = roleDTO.Name });

            if (result.Succeeded)
            {
                var identityRole = await roleManager.FindByNameAsync(roleDTO.Name);
                return CustomResult("Role Successfully Created", identityRole, HttpStatusCode.Created);
            }

            return CustomResult("Role Not Created");

        }

        [HttpGet("ManagePermissions/{id:Guid}")]
        public async Task<IActionResult> ManagePermissions(string id)
        {
            var role = await roleManager.FindByIdAsync(id);


            if (role == null)
                return CustomResult($"There is no Role with ID {id}", System.Net.HttpStatusCode.NotFound);

            var allPossiblePermissions = Permissions.GenerateAllAvailablePermissions();

            var roleClaims = await roleManager.GetClaimsAsync(role);

            var allPermissions = allPossiblePermissions.Select(p => new CheckedPermissionsDTO { PermissionName = p }).ToList();

            foreach(var permission in allPermissions)
            {
                if (roleClaims.Any(c => c.Type == "Permission" && c.Value == permission.PermissionName))
                    permission.IsSelected = true;
            }

            var rolePermissionDTO = new RolePermissionsDTO() { Name = role.Name,
                RoleID = role.Id,
               Permissions = allPermissions
            };

            return CustomResult(rolePermissionDTO);
        }

        [HttpPut("AssignPermissions")]
        public async Task<IActionResult> updatePermissions(RolePermissionsDTO rolePermissionsDTO)
        {
            var role = await roleManager.FindByIdAsync(rolePermissionsDTO.RoleID);


            if (role == null)
                return CustomResult($"There is no Role with ID {rolePermissionsDTO.RoleID}", System.Net.HttpStatusCode.BadRequest);

            var allRoleclaims = await roleManager.GetClaimsAsync(role);

            foreach (var claim in allRoleclaims)
                await roleManager.RemoveClaimAsync(role, claim);


            foreach (var permission in rolePermissionsDTO.Permissions)
            {
                if (permission.IsSelected)
                    await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission.PermissionName));
            }

            return CustomResult("Permissions Successfully updated");
        }
    }
}
