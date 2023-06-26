using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{
    public class RegisterUserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public ICollection<IFormFile>? UploadedImages { get; set; }
    }




    public record UserResponce(
        string Id,
        string UserName,
        string FirstName,
        string LastName,
        string Email,
        string? Address,
        string? CreditCardNumber,
        DateTime? LastUpdatedOn,
        string? PhoneNumber,
        List<string> ImageUrls

        );
}




