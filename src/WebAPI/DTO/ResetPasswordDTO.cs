using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class ResetPasswordDTO
    {
            public string Token { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Required*")]
            [RegularExpression(@"^(?=.*[!@#$%^&*()])(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$", ErrorMessage = "Password must contain upper, lower characters, numbers and special characters")]

            [MinLength(8, ErrorMessage = "Must be 8 characters at least")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Required*")]
            [Compare(nameof(Password), ErrorMessage = "Not matched")]
            [DataType(DataType.Password)]
            public string ConfirmedPassword { get; set; } = string.Empty;
        
    }
}
