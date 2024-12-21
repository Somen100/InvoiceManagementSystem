using System.ComponentModel.DataAnnotations;

namespace InvoiceMgmt.API.DTO
{
    public class InvoiceItemCreateDTO
    {
        public int InvoiceId { get; set; }  // The product being added to the invoice
        [Required]
        public int ProductId { get; set; }  // The product being added to the invoice.

        [Required]
        public int Quantity { get; set; }   // Quantity of the product.

        public bool IsActive { get; set; }  // Optional, default could be true.
    }
}
