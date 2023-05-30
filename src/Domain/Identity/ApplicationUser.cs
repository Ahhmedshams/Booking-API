using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastUpdatedById { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }
}
