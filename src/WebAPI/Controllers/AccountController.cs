using Application.DTO;
using AutoMapper;
using CoreApiResponse;
using Domain.Identity;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
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

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid email confirmation link");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("Invalid email confirmation link");
            }
            var DecodingResetToken = WebEncoders.Base64UrlDecode(token);
            var ValidToken = Encoding.UTF8.GetString(DecodingResetToken);
            var result = await userManager.ConfirmEmailAsync(user, ValidToken);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully");
            }
            else
            {
                return BadRequest("Unable to confirm email");
            }
        }




        [HttpPost("login")]
        [Authorize(Policy = "GuestUser")]

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
            if (Email == null || !ModelState.IsValid)
                return BadRequest("Check your email input");
            var Result = await accountRepo.ForgetPasswordAsync(Email);
            if (Result)
                return CustomResult("If your email matches one of our registerd account, we will send and email with resetting password steps");
            else
                return CustomResult("Something went wrong. Please try again later", System.Net.HttpStatusCode.ServiceUnavailable);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ConfirmResetPasswordAsync(ResetPasswordDTO ResetPasswordDto)
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


        [HttpGet("GetUser/{id:Guid}")]
        public async Task<IActionResult> GetById(string? id)
        {
            if (id == null)
                return CustomResult($"Need To provide Id {id}", HttpStatusCode.NotFound);

            var user = await accountRepo.GetByID(id);
            if (user == null)
                return CustomResult($"No User Type  Available With id==> {id}", HttpStatusCode.NotFound);

            var Result = mapper.Map<UserResponce>(user);


            return CustomResult(Result);
        }

        [HttpPatch("{Id:Guid}")]
        public async Task<IActionResult> EditUser(string Id, EditUserDTO user)
        {

            try
            {
                var AppUser = mapper.Map<ApplicationUser>(user);
                await accountRepo.EditAsync(Id,AppUser);
            }
            catch (Exception ex)
            {
                return CustomResult($"{ex.Message}", System.Net.HttpStatusCode.BadRequest);
            }
            return CustomResult();

        }

       
    }
}
