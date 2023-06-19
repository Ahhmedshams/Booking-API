using Domain.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<JwtSecurityToken> Login(ApplicationUser user);
        Task<object> Register(ApplicationUser _user, string password);
        Task<IdentityResult> ChangeEmailAsync(string OldEmail, string NewEmail, string Password);

        Task<IdentityResult> ChangePasswordAsync(string Email, string CurrentPassword, string NewPassword);

        Task<bool> ForgetPasswordAsync(string Email);

        Task<IdentityResult> ResetPasswordAsync(string Email, string Token, string NewPassword);

        Task<bool> ValidateTokenAsync(string Email, string Token);

        Task EditAsync(string id, ApplicationUser entity);

        Task<bool> IsExistAsync(string  id);
    }
}
