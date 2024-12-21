
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace InvoiceMgmt.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } 
        public decimal DiscountPercentage { get; set; } 

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.draft;
        public bool IsActive { get; set; }

       // [JsonIgnore] // Prevent serialization of navigation property
        public ICollection<InvoiceItem> InvoiceItems { get; set; } 
        public Customer Customer { get; set; }

    }

    public enum InvoiceStatus
    {
        draft,
        sent,
        paid,
        overdue
    }
}
