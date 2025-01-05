using InvoiceMgmt.BAL.IService.BulkUploads;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.DAL.IRepo.BulkUploads;
using InvoiceMgmt.Models;
using InvoiceMgmt.Models.Common;

namespace InvoiceMgmt.BAL.Service.BulkUploads
{
    public class BulkUploadsCustomersService : IBulkUploadsCustomersService
    {
        private readonly IUploadBulkCustomersRepo _repo;
        public BulkUploadsCustomersService(IUploadBulkCustomersRepo repo)
        {
            _repo = repo;
        }

        public Task AddBulkCustomersAsync(IEnumerable<CustomerBulkDTO> customers)
        {
            return _repo.AddBulkCustomersAsync(customers);
        }
    }
}
