using InvoiceMgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.DAL.IRepo
{
    public interface IInvoiceItemRepo
    {
        Task AddInvoiceItemsAsync(IEnumerable<InvoiceItem> invoiceItems);
        Task<InvoiceItem> GetInvoiceItemByIdAsync(int id);
        Task<IEnumerable<InvoiceItem>> GetInvoiceItemsAsync();
        Task<IEnumerable<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId);
        Task<InvoiceItem> CreateInvoiceItemAsync(InvoiceItem invoiceItem);
        Task<InvoiceItem> UpdateInvoiceItemAsync(InvoiceItem invoiceItem);
        Task DeleteInvoiceItemAsync(int id);
    }
}
