using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;

namespace InvoiceMgmt.BAL.Service
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly IAuditTrailRepo _repo;


        public AuditTrailService(IAuditTrailRepo repo)
        {
            _repo = repo;
        }

        public async Task<List<AuditTrail>> GetAuditLogsAsync()
        {
            return await _repo.GetAuditLogsAsync();
        }

        public async Task<AuditTrail> AddAuditLogAsync(AuditTrail auditTrail)
        {
            return await _repo.AddAuditLogAsync(auditTrail);
        }
    }
}
