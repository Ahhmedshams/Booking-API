using Infrastructure.Identity.EmailSettings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration config;
        private readonly IMailService mailService;

        public AccountRepository(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> roleManager,IConfiguration config, IMailService _MailService) 
        {
            userManager = _userManager;
            this.roleManager=roleManager;
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

                    var bodybuilder = new BodyBuilder();
                    bodybuilder.HtmlBody =
                    "</head>\r\n<body>\r\n  " +
                    "<div class=\"container\">\r\n    " +
                    "<p>Click the link below to Confirm your Email</p>\r\n" +
                    $"<a " +
                    $"style=\"display: inline-block; padding: .375rem .75rem; font-size: 1rem; font-weight: 400; line-height: 1.5; text-align: center; white-space: nowrap; vertical-align: middle; border: 1px solid #007bff; border-radius: .25rem; background-color: #007bff; color: #fff; text-decoration: none; text-decoration-style: none; text-decoration-color: none;\"" +
                    $" href={config["Server:Client"]}/auth/ConfirmEmail?userId={userFromDb.Id}&token={ValidEncodingConfirmToken}</a>" +
                    "<p>Best regards,</p>\r" +
                    "<p>Sona</p>\r\n  " +
                    "</div>\r\n</body>\r\n</html>";
                    var mailData = new MailData
                    {
                        EmailTo = userFromDb.Email,
                        EmailToName = userFromDb.UserName,
                        EmailSubject = "Confirm your email",
                        EmailBody = bodybuilder.HtmlBody
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
                               var identityRole = roleManager.Roles.FirstOrDefault(r => r.Name == role);
                                if (identityRole != null)
                    {
                        var roleClaims = await roleManager.GetClaimsAsync(identityRole);
                        if (roleClaims.Any(c => c.Type == "Permission"))
                        {
                            foreach(var claim in roleClaims)
                                myClaims.Add(claim);
                        }


                    }
                }
                    }

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]));
                    SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken myToken = new JwtSecurityToken(
                        expires: DateTime.Now.AddDays(25),
                        claims: myClaims,
                        signingCredentials: credentials,
                            issuer: config["JWT:Issuer"],
                         audience: config["JWT:Audience"]
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
                   /* var EncodingResetToken = Encoding.UTF8.GetBytes(ResetToken);
                    var ValidEncodingResetToken = WebEncoders.Base64UrlEncode(EncodingResetToken);*/ // To prevent special characters and make URL that will be generated valid

                    var message = new MimeMessage();

                    // Set the sender address
                    message.From.Add(new MailboxAddress(config["MailSettings:SenderName"],config["MailSettings:SenderEmail"]));

                    // Set the recipient address
                    message.To.Add(new MailboxAddress(user.UserName, Email));

                    // Set the subject
                    message.Subject = "Password Reset";

                    var bodybuilder = new BodyBuilder();
                    bodybuilder.HtmlBody =
                    "</head>\r\n<body>\r\n  " +
                    "<div class=\"container\">\r\n    " +
                    $"<p>Dear {user.UserName},</p>\r\n    " +
                    "<p>We received a request to reset your password. Please use the following token to reset your password:</p>\r\n    " +
                    $"<p>{ResetToken}</p>" +
                    "<p>Click the link below to reset your password</p>\r\n" +
                    $"<a " +
                    $"style=\"display: inline-block; padding: .375rem .75rem; font-size: 1rem; font-weight: 400; line-height: 1.5; text-align: center; white-space: nowrap; vertical-align: middle; border: 1px solid #007bff; border-radius: .25rem; background-color: #007bff; color: #fff; text-decoration: none; text-decoration-style: none; text-decoration-color: none;\"" +
                    $" href={config["Server:Client"]}/resetPassword>Reset Password</a>\r\n    " +
                    "<p>If you did not request a password reset, please ignore this email.</p>\r\n    " +
                    "<p>Best regards,</p>\r" +
                    "<p>Sona</p>\r\n  " +
                    "</div>\r\n</body>\r\n</html>";

                    message.Body = bodybuilder.ToMessageBody();

                    try
                    {
                        using (var client = new SmtpClient())
                        {
                            // Connect to the SMTP server
                            client.Connect(config["MailSettings:Server"], int.Parse(config["MailSettings:Port"]), SecureSocketOptions.SslOnConnect);

                            // Authenticate if required
                            client.Authenticate(config["MailSettings:UserName"], config["MailSettings:Password"]);

                            // Send the message
                            client.Send(message);

                            // Disconnect from the server
                            client.Disconnect(true);
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
                else return false;
            }
            return true ;
        }


        public async Task<IdentityResult> ResetPasswordAsync(string Email, string Token, string NewPassword)
        {
            var User = await userManager.FindByEmailAsync(Email);
            if (User != null)
            {
                /*var DecodingResetToken = WebEncoders.Base64UrlDecode(Token);
                var ValidToken = Encoding.UTF8.GetString(DecodingResetToken);*/
                var Result = await userManager.ResetPasswordAsync(User, Token, NewPassword);
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



        public async Task<ApplicationUser> GetByID(string ID)
        {
            var users = userManager.Users.Include(u=>u.Images);
            foreach(var user in users)
            {
                if(user.Id== ID)
                    return user;
            }
            return null;
        }



        public async Task EditAsync(string id, ApplicationUser entity)
        {
            var FoundUser = await userManager.FindByIdAsync(id);
            if (FoundUser != null)
            {
                FoundUser.FirstName = entity.FirstName ?? FoundUser.FirstName;
                FoundUser.LastName = entity.LastName ?? FoundUser.LastName;
                FoundUser.Email = entity.Email ?? FoundUser.Email;
                FoundUser.UserName = entity.UserName ?? FoundUser.UserName;
                FoundUser.PhoneNumber = entity.PhoneNumber ?? FoundUser.PhoneNumber;
                FoundUser.CreditCardNumber = entity.CreditCardNumber ?? FoundUser.CreditCardNumber;
                FoundUser.Address = entity.Address ?? FoundUser.Address;

                var result = await userManager.UpdateAsync(FoundUser);

                if (!result.Succeeded)
                {
                    foreach(var er in result.Errors)
                    {
                        throw new Exception(er.Description);
                    }
                    
                }
            };
        }

        public async Task<bool> IsExistAsync(string id)
        {
          var user = await  userManager.FindByIdAsync(id);
            if(user != null)
                return true;
            else
                return false;
        }
    }
}
