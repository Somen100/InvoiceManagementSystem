using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.DAL.IRepo.BulkUploads;
using InvoiceMgmt.Models;
using InvoiceMgmt.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace InvoiceMgmt.DAL.Repo.BulkUploads
{
    public class UploadBulkCustomersRepo : IUploadBulkCustomersRepo
    {
        private readonly InvoiceDbContext _context;
        public UploadBulkCustomersRepo(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task AddBulkCustomersAsync(IEnumerable<CustomerBulkDTO> customers)
        {
            // Map CustomerBulkDTO to Customer
            var mappedCustomers = customers.Select(c => new Customer
            {
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                IsActive = true // Set the default value or add logic to determine IsActive
            }).ToList();

            // Disable change tracking for performance optimization
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Add mapped customers in bulk
                await _context.Customers.AddRangeAsync(mappedCustomers);

                // Save changes and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Log exception and rollback transaction
                Console.WriteLine($"Error saving customers: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                // Re-enable change tracking
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }


    }
}
