namespace WebAPI.DTO
{
    public class ApplicationUserDTO
    {
        
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }

        public string UserName { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

    }
}
