using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.Service
{
    public class ProductRepo : IProductRepo
    {
        private readonly InvoiceDbContext _context;
        public ProductRepo(InvoiceDbContext context)
        {
            this._context = context;
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var record = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (record == null)
            {
                throw new Exception("Product Not Found");
            }
            record.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p=>p.ProductId == id);
        }

        public async Task UpdateProductAsync(int id, Product product)
        {
            var record = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (record == null)
            {
                throw new Exception("Product Not Found");
            }
            record.Name = product.Name;
            record.Description = product.Description;
            record.UnitPrice = product.UnitPrice;
            record.IsActive = true;
            
            await _context.SaveChangesAsync();
        }
    }
}
