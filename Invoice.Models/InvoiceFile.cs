using System;

namespace InvoiceMgmt.Models
{
    public class InvoiceFile
    {
        public int InvoiceFileId { get; set; } // Primary Key
        public int InvoiceId { get; set; } // Foreign Key to Invoice.
        public string FileName { get; set; }  // File name (e.g., "invoice123.pdf")
        public string FilePath { get; set; } // Path to the uploaded file
        public DateTime UploadDate { get; set; } // When the file was uploaded

        // Navigation properties
        public Invoice Invoice { get; set; }
    }
}
