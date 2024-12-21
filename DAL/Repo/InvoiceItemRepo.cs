using InvoiceMgmt.Models;
using InvoiceMgmt.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceMgmt.DAL.IRepo;

namespace InvoiceMgmt.DAL.Repo
{
    public class InvoiceItemRepo : IInvoiceItemRepo
    {
        private readonly InvoiceDbContext _context;

        public InvoiceItemRepo(InvoiceDbContext context)
        {
            _context = context;
        }


        public async Task AddInvoiceItemsAsync(IEnumerable<InvoiceItem> invoiceItems)
        {
            _context.InvoiceItems.AddRange(invoiceItems);
            await _context.SaveChangesAsync();
        }

        public async Task<InvoiceItem> GetInvoiceItemByIdAsync(int id)
        {
            return await _context.InvoiceItems
                                 .Include(ii => ii.Invoice)
                                 .Include(ii => ii.Product)
                                 .FirstOrDefaultAsync(ii => ii.InvoiceItemId == id);
        }

        public async Task<IEnumerable<InvoiceItem>> GetInvoiceItemsAsync()
        {
            return await _context.InvoiceItems
                                 .Include(ii => ii.Invoice)
                                 .Include(ii => ii.Product)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.InvoiceItems
                                 .Include(ii => ii.Invoice)
                                 .Include(ii => ii.Product)
                                 .Where(ii => ii.InvoiceId == invoiceId)
                                 .ToListAsync();
        }

        public async Task<InvoiceItem> CreateInvoiceItemAsync(InvoiceItem invoiceItem)
        {
            _context.InvoiceItems.Add(invoiceItem);
            await _context.SaveChangesAsync();
            return invoiceItem;
        }

        public async Task<InvoiceItem> UpdateInvoiceItemAsync(InvoiceItem invoiceItem)
        {
            _context.InvoiceItems.Update(invoiceItem);
            await _context.SaveChangesAsync();
            return invoiceItem;
        }

        public async Task DeleteInvoiceItemAsync(int id)
        {
            var record = await _context.InvoiceItems.FirstOrDefaultAsync(p => p.InvoiceItemId == id);
            if (record == null)
            {
                throw new Exception("InvoiceItem Not Found");
            }
            record.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
