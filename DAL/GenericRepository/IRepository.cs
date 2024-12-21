using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.DAL.GenericRepository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        // Other methods like Update, Delete, etc., can be added here.
    }
}
