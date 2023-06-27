using Application.DTO;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Domain.Identity;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Xml.Linq;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly AccountRepository accountRepo;
        private readonly UploadImage _uploadImage;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;
        public AccountController(UserManager<ApplicationUser> _userManager,
            IMapper _mapper, AccountRepository accountRepo, UploadImage uploadImage, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions)
        {
            userManager = _userManager;
            mapper = _mapper;
            this.accountRepo = accountRepo;
            this._uploadImage = uploadImage;
            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] SieveModel sieveModel)
        {
            var users = await userManager.Users.Include(u=>u.Images).ToListAsync();
            List < ApplicationUserDTO> usersDto= new List <ApplicationUserDTO>();
            foreach (var user in users)
            {
                var userDto = new ApplicationUserDTO()
                {
                    Id=user.Id,
                    Email=user.Email,
                    FirstName=user.FirstName,
                    LastName=user.LastName,
                    UserName= user.UserName,
                    ImageUrls=user.Images
                };
                usersDto.Add(userDto);
            }

            var FilteredResources = _sieveProcessor.Apply(sieveModel, usersDto.AsQueryable());
            //List<ApplicationUserDTO> userDTOs = mapper.Map<List<ApplicationUserDTO>>(users);
            return CustomResult(FilteredResources);
        }



        [HttpPost("register")]
        /*  [ServiceFilter(typeof(ValidationFilterAttribute))]*/
        public async Task<IActionResult> Register(RegisterUserDto _user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = mapper.Map<ApplicationUser>(_user);
                if (_user.UploadedImages != null && _user.UploadedImages.Count != 0)
                {
                    var entityType = "UserImage";
                    var images = await _uploadImage.UploadToCloud(_user.UploadedImages, entityType);

                    if (images != null && images.Any())
                    {
                        var userImages = images.OfType<UserImage>().ToList();
                        user.Images = userImages;
                    }
                }

                object result = await accountRepo.Register(user, _user.Password);

                if (result is IdentityResult)
                {
                    return CustomResult("Created Successfully");
                }
                else if (result is IEnumerable<IdentityError> errorList)
                {
                    return CustomResult(errorList, HttpStatusCode.BadRequest);
                }
            }
            return CustomResult(ModelState, HttpStatusCode.BadRequest);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return CustomResult("Invalid email confirmation link", HttpStatusCode.BadRequest);
            }

            var user = await userManager.FindByIdAsync(userId);
            //if (user == null)
            //{
            //    return CustomResult("Invalid email confirmation link", HttpStatusCode.BadRequest);
            //}

            var DecodingResetToken = WebEncoders.Base64UrlDecode(token);
            var ValidToken = Encoding.UTF8.GetString(DecodingResetToken);
            var result = await userManager.ConfirmEmailAsync(user, ValidToken);
            if (result.Succeeded)
            {
                return CustomResult("Email confirmed successfully");
            }
            else
            {
                return CustomResult("Unable to confirm email", HttpStatusCode.BadRequest);
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

                    return CustomResult(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(myToken),
                        expiration = myToken.ValidTo
                    });
                }
            }
            return CustomResult(ModelState, HttpStatusCode.BadRequest);
        }

        [HttpPost("Change/Email")]
        public async Task<IActionResult> ChangeEmailAsync(ChangeEmailDTO ChangeEmailDto)
        {
            if (ModelState.IsValid)
            {
                var Result = await accountRepo.ChangeEmailAsync(ChangeEmailDto.OldEmail, ChangeEmailDto.NewEmail, ChangeEmailDto.Password);
                if (Result.Succeeded)
                {
                    return CustomResult("Email has been changed successfully");
                }
                return CustomResult("Your email or password incorrect", HttpStatusCode.Unauthorized);
            }
            else
            {
                return CustomResult("Email Is Invalid", HttpStatusCode.BadRequest);
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
                    return CustomResult("Password has been changed successfully");
                }
                return CustomResult("Your email or password incorrect", HttpStatusCode.Unauthorized);
            }

            return CustomResult(ModelState.Values, HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync([EmailAddress] string Email)
        {
            if (Email == null || !ModelState.IsValid)
                return CustomResult("Check your email input", HttpStatusCode.BadRequest);
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
                        return CustomResult("Password Has Been Reset Successfully");
                    }
                    string Errors = string.Empty;
                    foreach (var Error in Result.Errors)
                    {
                        Errors += Error.Description.Substring(0, Error.Description.Length - 1) + ", ";
                    }
                    return CustomResult(Errors.Substring(0, Errors.Length - 2), HttpStatusCode.BadRequest);
                }
                return CustomResult(ModelState.Values, HttpStatusCode.BadRequest);
            }
            return CustomResult("All Fields Are Required", HttpStatusCode.BadRequest);
        }


        [HttpGet("GetUser/{id:Guid}")]
        public async Task<IActionResult> GetById(string? id)
        {
            if (id == null)
                return CustomResult($"Need To provide Id {id}", HttpStatusCode.NotFound);

            var user = await accountRepo.GetByID(id);
            if (user == null)
                return CustomResult($"No User Type  Available With id==> {id}", HttpStatusCode.NotFound);

            // var Result = mapper.Map<UserResponce>(user);
            var result = new UserRespDTO()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                ImageUrls = user.Images,
                Address=user.Address,
                CreditCardNumber =user.CreditCardNumber,
                PhoneNumber = user.PhoneNumber
            };

            return CustomResult(result);
        }

        [HttpPatch("{Id:Guid}")]
        public async Task<IActionResult> EditUser(string Id, EditUserDTO user)
        {

            try
            {
                var AppUser = mapper.Map<ApplicationUser>(user);
                await accountRepo.EditAsync(Id,AppUser);
				return CustomResult(user);
			}
            catch (Exception ex)
            {
                return CustomResult($"{ex.Message}", System.Net.HttpStatusCode.BadRequest);
            }
            

        }

       
    }
}
