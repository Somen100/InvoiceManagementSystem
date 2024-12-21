using InvoiceMgmt.Models;

namespace InvoiceMgmt.BAL.IService
{
    public interface IAuditTrailService
    {
        Task<List<AuditTrail>> GetAuditLogsAsync();
        Task<AuditTrail> AddAuditLogAsync(AuditTrail auditTrail);
    }
}
