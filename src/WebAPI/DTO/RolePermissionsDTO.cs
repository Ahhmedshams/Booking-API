namespace WebAPI.DTO
{
    public class RolePermissionsDTO
    {
        
        public string RoleID { get; set; }
        public string Name { get; set; }
        public List<CheckedPermissionsDTO> Permissions { get; set; } = new List<CheckedPermissionsDTO>();

    }

    public class CheckedPermissionsDTO
    {
        public string PermissionName { get; set; }

        public bool IsSelected { get; set; }
    }
}
