using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.Models
{
    public class AuditTrail
    {
        [Key]
        public int AuditId { get; set; }

        [Required]
        public string TableName { get; set; }

        [Required]
        public string Operation { get; set; } // e.g., Insert, Update, Delete

        [Required]
        public string Changes { get; set; } // Serialized JSON of changes

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string PerformedBy { get; set; }
    }
}
