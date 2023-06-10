using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class ChangeEmailDTO
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string OldEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string NewEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required")]
        public string Password { get; set; } = string.Empty;
    }
}