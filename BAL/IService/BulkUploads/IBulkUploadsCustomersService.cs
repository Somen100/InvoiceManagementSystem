using InvoiceMgmt.Models;
using InvoiceMgmt.Models.Common;

namespace InvoiceMgmt.BAL.IService.BulkUploads
{
    public interface IBulkUploadsCustomersService
    {
        Task AddBulkCustomersAsync(IEnumerable<CustomerBulkDTO> customers);
    }
}
