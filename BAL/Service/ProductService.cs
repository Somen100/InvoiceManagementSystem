using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _repo;
        public ProductService(IProductRepo repo)
        {
            this._repo = repo;
        }

        public Task AddProductAsync(Product product)
        {
            return _repo.AddProductAsync(product);
        }

        public Task DeleteProductAsync(int id)
        {
          return _repo.DeleteProductAsync(id);
        }

        public Task<IEnumerable<Product>> GetAllProductAsync()
        {
            return _repo.GetAllProductAsync();
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            return _repo.GetProductByIdAsync(id);
        }

        public Task UpdateProductAsync(int id, Product product)
        {
           return _repo.UpdateProductAsync(id, product);
        }
    }
}
