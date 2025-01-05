namespace InvoiceMgmt.API.DTO
{
    public class UserCreateDTO
    {
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        // Foreign key for RoleMaster
        public int? RoleId { get; set; }
    }
}
