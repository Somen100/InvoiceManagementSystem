using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Http;
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


        public async Task<Invoice> AddOnlyInvoiceAsync(Invoice invoice, List<IFormFile>? uploadedFiles)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Process the uploaded files and associate them with the invoice
            if (uploadedFiles != null && uploadedFiles.Count > 0)
            {
                var uploadedInvoiceFiles = new List<InvoiceFile>();

                foreach (var file in uploadedFiles)
                {
                    // Define the file path where files will be stored
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Files", "invoice-files");

                    // Check if the folder exists, create it if not
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate a unique file name for each uploaded file
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file to the file system
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Create InvoiceFile objects for each uploaded file
                    var invoiceFile = new InvoiceFile
                    {
                        InvoiceId = invoice.InvoiceId,
                        FileName = fileName,  
                        FilePath = filePath,  
                        UploadDate = DateTime.Now 
                    };
                    uploadedInvoiceFiles.Add(invoiceFile);
                    await _context.SaveChangesAsync();
                }
                // Add the invoice files to the database in bulk
                await _context.InvoiceFiles.AddRangeAsync(uploadedInvoiceFiles);
                await _context.SaveChangesAsync(); // Save changes
            }
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
             string? invoiceNumber = null, string? status="", int? customerId = null)
        {
            status = status ?? "draft";
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




