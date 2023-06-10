using Infrastructure.Migrations;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace WebAPI.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly AccountRepository accountRepo;

        public ResetPasswordController(AccountRepository accountRepo)
        {
            this.accountRepo = accountRepo;
        }

        [Route("ResetPassword")]
        [HttpGet]
        public async Task<IActionResult> ResetPasswordPage([FromQuery] string Email, [FromQuery] string Token)
        {
            ViewBag.Email = Email;
            ViewBag.Token = Token;
            if (await accountRepo.ValidateTokenAsync(Email, Token))
            {
                return View();
            }
            return Content("Not Found");
        }
    }
}

