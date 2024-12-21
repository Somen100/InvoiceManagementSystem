using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceMgmt.Models
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal TotalPrice { get; set; } // Calculated based on Product.UnitPrice * Quantity

        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }  // Add this line for the navigation property
    }
}
