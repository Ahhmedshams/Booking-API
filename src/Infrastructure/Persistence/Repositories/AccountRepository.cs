using Infrastructure.Identity.EmailSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
                var userFromDb = await userManager.FindByEmailAsync(_user.Email);
                if (userFromDb != null)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(userFromDb);
                    // Send the confirmation email
                    var EncodingConfirmToken = Encoding.UTF8.GetBytes(token);
                    var ValidEncodingConfirmToken = WebEncoders.Base64UrlEncode(EncodingConfirmToken);

                    var mailData = new MailData
                    {
                        EmailTo = userFromDb.Email,
                        EmailToName = userFromDb.UserName,
                        EmailSubject = "Confirm your email",
                        EmailBody = $"Please confirm your email address by clicking this link:\n {config["Server:URL"]}/ConfirmEmail?userId={userFromDb.Id}&token={ValidEncodingConfirmToken}"
                    };
                    mailService.SendMail(mailData);
                    
                }
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

        public async Task<bool> ForgetPasswordAsync(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                string ResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                if (ResetToken != null)
                {
                    var EncodingResetToken = Encoding.UTF8.GetBytes(ResetToken);
                    var ValidEncodingResetToken = WebEncoders.Base64UrlEncode(EncodingResetToken); // To prevent special characters and make URL that will be generated valid
                    MailData mailData = new MailData()
                    {
                        EmailTo = Email,
                        EmailToName = user.UserName,
                        EmailSubject = "Password Reset",
                        EmailBody =
                        //$"Click to the following link to reset your password \n {config["Server:URL"]}/ResetPassword?Email={Email}&Token={ValidEncodingResetToken}",
                        $"Dear {user.UserName},\r\n" +
                        "We received a request to reset your password. Please use the following token to reset your password:\r\n\n" +
                        $"{ValidEncodingResetToken} \r\n\n" +
                        "Click the link below to reset your password\r\n" +
                        $"{config["Server:Client"]}/resetPassword\r\n" +
                        "If you did not request a password reset, please ignore this email.\r\n\n" +
                        "Best regards,\r\n" +
                        "Sona\r\n"
                    };
                    if (mailService.SendMail(mailData))
                    {
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            return true ;
        }

        /*public async Task<string> ConfirmEailAsync(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if(user != null)
            {

            }
        }*/

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
