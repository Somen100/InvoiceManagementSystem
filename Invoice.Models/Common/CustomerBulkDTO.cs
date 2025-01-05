namespace InvoiceMgmt.Models.Common
{
    public class CustomerBulkDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
