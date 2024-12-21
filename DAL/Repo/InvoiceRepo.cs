using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceMgmt.DAL.Repo
{
    public class InvoiceRepo : IInvoiceRepo
    {
        private readonly InvoiceDbContext _context;
        public InvoiceRepo(InvoiceDbContext context)
        {
            _context = context;
        }


        public async Task<Invoice> AddOnlyInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }
        public async Task<Invoice> UpdateOnlyInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task DeleteInvoice(int id)
        {
            var record = await _context.Invoices.FirstOrDefaultAsync(u => u.InvoiceId == id);
            if (record == null)
            {
                throw new KeyNotFoundException("Invoice not found");
            }
            record.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoice(int pageNumber = 1, int pageSize = 10,
             string? invoiceNumber = null, string? status = "draft", int? customerId = null)
        {
            // Start with the base query
            IQueryable<Invoice> query = _context.Invoices
                .Include(i => i.InvoiceItems)
                .Include(i => i.Customer);

            // Apply filters if provided
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                query = query.Where(i => i.InvoiceNumber.Contains(invoiceNumber));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<InvoiceStatus>(status, true, out var parsedStatus))
                {
                    // If parsing is successful, filter by the parsed status
                    query = query.Where(i => i.Status == parsedStatus);
                }
                else
                {
                    // If the status is invalid, handle the error (return an empty list or throw an exception)
                    throw new ArgumentException($"Invalid status: {status}");
                }
            }

            if (customerId.HasValue)
            {
                query = query.Where(i => i.CustomerId == customerId);
            }

            // Apply pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Return the paginated and filtered result
            return await query.ToListAsync();
        }

        public async Task<Invoice> GetInvoiceById(int id)
        {
            return await _context.Invoices.FirstOrDefaultAsync(u=>u.InvoiceId == id);
        }

        public async Task<Invoice> GetInvoiceByNumber(string invoiceNo)
        {
            return await _context.Invoices.FirstOrDefaultAsync(u => u.InvoiceNumber == invoiceNo);
        }
        public async Task<Invoice> CreateInvoice(Invoice invoice, List<InvoiceItem> invoiceItems)
        {
            // Add Invoice to the Invoice table
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(); // Save to get the Invoice ID

            // Add InvoiceItems to the InvoiceItem table
            foreach (var item in invoiceItems)
            {
                item.InvoiceId = invoice.InvoiceId;  // Set the foreign key relationship
                _context.InvoiceItems.Add(item);
            }
            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task<Invoice> UpdateInvoice(int id, Invoice updatedInvoice, List<InvoiceItem> updatedInvoiceItems)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                throw new Exception("Invoice Not Found");
            }

            // Update invoice properties
            invoice.InvoiceNumber = updatedInvoice.InvoiceNumber;
            invoice.Status = updatedInvoice.Status;
            invoice.IsActive = updatedInvoice.IsActive;
            invoice.DiscountPercentage = updatedInvoice.DiscountPercentage;
            invoice.CustomerId = updatedInvoice.CustomerId;


            // Update or add InvoiceItems
            foreach (var item in updatedInvoiceItems)
            {
                item.InvoiceId = invoice.InvoiceId;  // Ensure the foreign key is set

                var existingItem = await _context.InvoiceItems
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId && i.InvoiceItemId == item.InvoiceItemId);

                if (existingItem != null)
                {
                    // Update the existing InvoiceItem
                    existingItem.ProductId = item.ProductId;
                    existingItem.Quantity = item.Quantity;
                }
                else
                {
                    // Add the new InvoiceItem
                    _context.InvoiceItems.Add(item);
                }
            }

            await _context.SaveChangesAsync();
            return invoice;
        }

    }
}




