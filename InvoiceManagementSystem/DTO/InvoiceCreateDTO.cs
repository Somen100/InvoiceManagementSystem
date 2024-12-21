using InvoiceMgmt.Models;
using System.ComponentModel.DataAnnotations;

namespace InvoiceMgmt.API.DTO
{
    public class InvoiceCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public decimal DiscountPercentage { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = InvoiceStatus.draft.ToString(); 

        [Required]
        public ICollection<InvoiceItemCreateDTO> InvoiceItems { get; set; } // Item details to create the invoice.
    }
}
