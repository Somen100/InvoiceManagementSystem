using System.ComponentModel.DataAnnotations;

namespace InvoiceMgmt.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        public ICollection<Invoice> Invoices { get; set; }

        public bool IsActive { get; set; }
    }
}
