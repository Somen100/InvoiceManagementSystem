using InvoiceMgmt.DAL.GenericRepository;
using InvoiceMgmt.Models;

namespace InvoiceMgmt.DAL.IRepo
{
    public interface IAuditTrailRepo : IRepository<AuditTrail>
    {
        Task<List<AuditTrail>> GetAuditLogsAsync();
        Task<AuditTrail> AddAuditLogAsync(AuditTrail auditTrail);
    }
}
