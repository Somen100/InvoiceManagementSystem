using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;

namespace InvoiceMgmt.BAL.Service
{
    public class InvoiceItemService : IInvoiceItemService
    {
        private readonly IInvoiceItemRepo _invoiceItemRepo;
        public InvoiceItemService(IInvoiceItemRepo invoiceItemRepo)
        {
            _invoiceItemRepo = invoiceItemRepo;
        }

        public Task AddInvoiceItemsAsync(IEnumerable<InvoiceItem> invoiceItems)
        {
            return _invoiceItemRepo.AddInvoiceItemsAsync(invoiceItems);
        }

        public Task<InvoiceItem> CreateInvoiceItemAsync(InvoiceItem invoiceItem)
        {
           return _invoiceItemRepo.CreateInvoiceItemAsync(invoiceItem);
        }

        public Task DeleteInvoiceItemAsync(int id)
        {
            return _invoiceItemRepo.DeleteInvoiceItemAsync(id);
        }

        public Task<InvoiceItem> GetInvoiceItemByIdAsync(int id)
        {
            return _invoiceItemRepo.GetInvoiceItemByIdAsync(id);
        }
        public Task<IEnumerable<InvoiceItem>> GetInvoiceItemsAsync()
        {
           return _invoiceItemRepo.GetInvoiceItemsAsync();
        }

        public Task<IEnumerable<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId)
        {
           return _invoiceItemRepo.GetInvoiceItemsByInvoiceIdAsync(invoiceId);
        }

        public Task<InvoiceItem> UpdateInvoiceItemAsync(InvoiceItem invoiceItem)
        {
            return _invoiceItemRepo.UpdateInvoiceItemAsync(invoiceItem);
        }
    }
}
