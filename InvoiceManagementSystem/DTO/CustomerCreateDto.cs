using System.ComponentModel.DataAnnotations;

namespace InvoiceMgmt.API.DTO
{
    public class CustomerCreateDto
    {
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

        // Optional, can be set to true in controller or service layer.
        public bool IsActive { get; set; } = true;
    }
}
