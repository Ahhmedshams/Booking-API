using Application.DTO;
using AutoMapper;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration config;

        public AccountController(UserManager<ApplicationUser> _userManager,IMapper _mapper,IConfiguration config) {
            userManager = _userManager;
            mapper = _mapper;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto _user)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser user = mapper.Map<ApplicationUser>(_user);
                IdentityResult result = await userManager.CreateAsync(user,_user.Password);

                if(result.Succeeded)
                {
                    return Ok("Created Successfully");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto _user)
        {
            if(ModelState.IsValid)
            {
               ApplicationUser user = await userManager.FindByEmailAsync(_user.Email);

                if (user != null && await userManager.CheckPasswordAsync(user ,_user.Password))
                {
                    List<Claim> myClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };

                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        foreach(var role in roles)
                        {
                            myClaims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]));
                    SigningCredentials credentials =new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken myToken = new JwtSecurityToken(
                        expires:DateTime.Now.AddDays(25),
                        claims:myClaims,
                        signingCredentials:credentials
                        );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(myToken),
                        expiration=myToken.ValidTo
                    });
                }
            }
            return BadRequest("Invalid Password");
        }
    }
}
