namespace InvoiceMgmt.Models
{
    public class RoleMaster
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        // Navigation property for related users
        public ICollection<User> Users { get; set; }
    }
}
