using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "The email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Current Password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "The New Password is required")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
