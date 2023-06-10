﻿using AutoMapper;
using Azure.Core;
using Infrastructure.Identity;
using Infrastructure.Identity.EmailSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly IMailService mailService;

        public AccountRepository(UserManager<ApplicationUser> _userManager,IConfiguration config, IMailService _MailService) 
        {
            userManager = _userManager;
            this.config = config;
            mailService = _MailService;
        }
        public async Task<object> Register(ApplicationUser _user, string password)
        {
            IdentityResult result = await userManager.CreateAsync(_user, password);
            if (result.Succeeded)
            {
               /* var token = await userManager.GenerateEmailConfirmationTokenAsync(_user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                    new { userId = _user.Id, token = token }, Request.);*/
                return result;
            }
            else
                return result.Errors;
        }

        public async Task<JwtSecurityToken> Login(ApplicationUser user)
        {
                    List<Claim> myClaims = new List<Claim>
                    {
                        new Claim("Id", user.Id),
                        new Claim("Email", user.Email)
                    };

                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            myClaims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]));
                    SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken myToken = new JwtSecurityToken(
                        expires: DateTime.Now.AddDays(25),
                        claims: myClaims,
                        signingCredentials: credentials
                        );
                    return myToken; 
                
            }

        public async Task<IdentityResult> ChangeEmailAsync(string OldEmail, string NewEmail, string Password)
        {
            var user = await userManager.FindByEmailAsync(OldEmail);
            if (user != null)
            {
                var Result = await userManager.CheckPasswordAsync(user, Password);
                if (Result)
                {
                    string Token = await userManager.GenerateChangeEmailTokenAsync(user, NewEmail);
                    await userManager.ChangeEmailAsync(user, NewEmail, Token);
                    return IdentityResult.Success;
                }
            }
            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> ChangePasswordAsync(string Email, string CurrentPassword, string NewPassword)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                return await userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
            }
            return IdentityResult.Failed();
        }

        public async Task<string?> ForgetPasswordAsync(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                string ResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                if (ResetToken!=null)
                {
                    var EncodingResetToken = Encoding.UTF8.GetBytes(ResetToken);
                    var ValidEncodingResetToken = WebEncoders.Base64UrlEncode(EncodingResetToken); // To prevent special characters and make URL that will be generated valid
                    MailData mailData = new MailData()
                    {
                        EmailTo = Email,
                        EmailToName = user.UserName,
                        EmailSubject = "Reset Your Password",
                        EmailBody = $"Click to the following link to reset your password \n {config["Server:URL"]}/ResetPassword?Email={Email}&Token={ValidEncodingResetToken}",
                    };
                    if (mailService.SendMail(mailData))
                    {
                        return "If your email is found, you will receive a link to reset your password";
                    }
                }
            }
            return null;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string Email, string Token, string NewPassword)
        {
            var User = await userManager.FindByEmailAsync(Email);
            if (User != null)
            {
                var DecodingResetToken = WebEncoders.Base64UrlDecode(Token);
                var ValidToken = Encoding.UTF8.GetString(DecodingResetToken);
                var Result = await userManager.ResetPasswordAsync(User, ValidToken, NewPassword);
                return Result;
            }
            return IdentityResult.Failed(new IdentityError() { Code = "Email Not Found", Description = "This email is not found" });
        }

        public async Task<bool> ValidateTokenAsync(string Email, string Token)
        {
            var User = await userManager.FindByEmailAsync(Email);
            if (User != null)
            {
                var DecodingResetToken = WebEncoders.Base64UrlDecode(Token);
                var ValidToken = Encoding.UTF8.GetString(DecodingResetToken);
                return await userManager.VerifyUserTokenAsync(User, "Default", "ResetPassword", ValidToken);
            }
            return false;
        }
    }
}
