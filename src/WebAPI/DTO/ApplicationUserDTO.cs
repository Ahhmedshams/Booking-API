using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class ApplicationUserDTO
    {
        
        public string Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string FirstName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string LastName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Email { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string UserName { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public ICollection<UserImage> ImageUrls { get; set; }

    }
}
