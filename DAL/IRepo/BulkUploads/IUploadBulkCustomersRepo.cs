using InvoiceMgmt.Models.Common;

namespace InvoiceMgmt.DAL.IRepo.BulkUploads
{
    public interface IUploadBulkCustomersRepo
    {
        Task AddBulkCustomersAsync(IEnumerable<CustomerBulkDTO> customers);
    }
}
