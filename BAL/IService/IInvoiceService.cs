using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.IService
{
    public interface IInvoiceService
    {
        Task<Invoice> UpdateOnlyInvoiceAsync(Invoice invoice);
        Task<Invoice> AddOnlyInvoiceAsync(Invoice invoice, List<IFormFile>? uploadedFiles);
        Task<IEnumerable<Invoice>> GetAllInvoice(int pageNumber = 1, int pageSize = 10,
            string? invoiceNumber = null, string? status = null, int? customerId = null);
        Task<Invoice> GetInvoiceById(int id);
        Task<Invoice> GetInvoiceByNumber(string invoiceNo);
        Task<Invoice> UpdateInvoice(int id, Invoice invoice, List<InvoiceItem> updatedInvoiceItems);
        Task DeleteInvoice(int id);
        Task<Invoice> CreateInvoice(Invoice invoice, List<InvoiceItem> invoiceItems);
    }
}
