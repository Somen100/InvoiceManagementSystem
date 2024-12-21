using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceMgmt.DAL.Repo
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly InvoiceDbContext _context;
        public CustomerRepo(InvoiceDbContext context)
        {
            this._context = context;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
           var record = await _context.Customers.FirstOrDefaultAsync(p => p.CustomerId == id);
            if(record == null)
            {
                throw new Exception("Customer Not Found");
            }
            record.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(p => p.CustomerId == id);
        }

        public async Task UpdateCustomerAsync(int id, Customer customer)
        {
            var record = await _context.Customers.FirstOrDefaultAsync(p => p.CustomerId == id);
            if (record == null)
            {
                throw new Exception("Customer Not Found");
            }
            record.PhoneNumber = customer.PhoneNumber;
            record.Address = customer.Address;
            record.Email = customer.Email;
            record.IsActive = true;
            await _context.SaveChangesAsync();
        }
    }
}
