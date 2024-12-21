using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;

namespace InvoiceMgmt.BAL.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _repo;

        public CustomerService(ICustomerRepo repo)
        {
            this._repo = repo;
        }

        public Task AddCustomerAsync(Customer customer)
        {
            return _repo.AddCustomerAsync(customer);
        }

        public Task DeleteCustomerAsync(int id)
        {
            return _repo.DeleteCustomerAsync(id);
        }

        public Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
           return _repo.GetAllCustomersAsync();
        }

        public Task<Customer> GetCustomerByIdAsync(int id)
        {
            return _repo.GetCustomerByIdAsync(id);
        }

        public Task UpdateCustomerAsync(int id, Customer customer)
        {
           return _repo.UpdateCustomerAsync(id, customer);
        }
    }
}
