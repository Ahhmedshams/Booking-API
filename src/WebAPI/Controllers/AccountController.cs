using Application.DTO;
using AutoMapper;
using Domain.Identity;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly AccountRepository accountRepo;

        public AccountController(UserManager<ApplicationUser> _userManager, IMapper _mapper, AccountRepository accountRepo)
        {
            userManager = _userManager;
            mapper = _mapper;
            this.accountRepo = accountRepo;
        }

        [HttpPost("register")]
        /*  [ServiceFilter(typeof(ValidationFilterAttribute))]*/
        public async Task<IActionResult> Register(RegisterUserDto _user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = mapper.Map<ApplicationUser>(_user);
                object result = await accountRepo.Register(user, _user.Password);

                if (result is IdentityResult)
                {
                    return Ok("Created Successfully");
                }
                else if (result is IEnumerable<IdentityError> errorList)
                {
                    return BadRequest(errorList);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto _user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(_user.Email);

                if (user != null && await userManager.CheckPasswordAsync(user, _user.Password))
                {
                    JwtSecurityToken myToken = await accountRepo.Login(user);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(myToken),
                        expiration = myToken.ValidTo
                    });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Change/Email")]
        public async Task<IActionResult> ChangeEmailAsync(ChangeEmailDTO ChangeEmailDto)
        {
            if (ModelState.IsValid)
            {
                var Result = await accountRepo.ChangeEmailAsync(ChangeEmailDto.OldEmail, ChangeEmailDto.NewEmail, ChangeEmailDto.Password);
                if (Result.Succeeded)
                {
                    return Ok("Email has been changed successfully");
                }
                return Unauthorized("Your email or password incorrect");
            }
            else
            {
                return BadRequest("Email Is Invalid");
            }
        }
        [HttpPost("Change/Password")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO ChangePasswordDto)
        {
            if (ModelState.IsValid)
            {
                var Result = await accountRepo.ChangePasswordAsync(ChangePasswordDto.Email, ChangePasswordDto.CurrentPassword, ChangePasswordDto.NewPassword);
                if (Result.Succeeded)
                {
                    return Ok("Password has been changed successfully");
                }
                return Unauthorized("Your email or password incorrect");
            }

            return BadRequest(ModelState.Values);
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync([EmailAddress] string Email)
        {
            if (Email != null)
            {
                if (ModelState.IsValid)
                {
                    var Result = await accountRepo.ForgetPasswordAsync(Email);
                    return Ok(Result);
                }
                return BadRequest("Email IS Invalid");
            }
            return BadRequest("Email Is Required");
        }

        [HttpPost]
        [Route("ResetPassword")]
        private async Task<IActionResult> ConfirmResetPasswordAsync([FromForm] ResetPasswordDTO ResetPasswordDto)
        {
            if (ResetPasswordDto != null)
            {
                if (ModelState.IsValid)
                {
                    var Result = await accountRepo.ResetPasswordAsync(ResetPasswordDto.Email, ResetPasswordDto.Token, ResetPasswordDto.Password);
                    if (Result.Succeeded)
                    {
                        return Ok("Password Has Been Reset Successfully");
                    }
                    string Errors = string.Empty;
                    foreach (var Error in Result.Errors)
                    {
                        Errors += Error.Description.Substring(0, Error.Description.Length - 1) + ", ";
                    }
                    return BadRequest(Errors.Substring(0, Errors.Length - 2));
                }
                return BadRequest(ModelState.Values);
            }
            return BadRequest("All Fields Are Required");
        }
    }
}
