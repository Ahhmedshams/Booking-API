namespace WebAPI.DTO
{
    public class UserRolesDTO
    {
        
        public string UserID { get; set; }
        public string? Name { get; set; }
        public List<CheckedRoleDTO> Roles { get; set; } = new List<CheckedRoleDTO>();

    }

    public class CheckedRoleDTO
    {
        public string RoleName { get; set; }

        public bool IsSelected { get; set; }
    }
}
