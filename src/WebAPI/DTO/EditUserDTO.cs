using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class EditUserDTO
    {
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must only contain letters.")]
        public string? FirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must only contain letters.")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [RegularExpression(@"^01[0125][0 - 9]{ 8}$", ErrorMessage = "Not valid phone Number")]
        public string? PhoneNumber { get; set; }

        public string? UserName { get; set; }
        public string? Address { get; set; }

    }
}

