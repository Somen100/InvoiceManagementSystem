using System.ComponentModel.DataAnnotations;

namespace InvoiceMgmt.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public decimal GSTPercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
