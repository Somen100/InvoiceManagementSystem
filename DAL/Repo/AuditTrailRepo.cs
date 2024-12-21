using InvoiceMgmt.DAL.Data;
using InvoiceMgmt.DAL.GenericRepository;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceMgmt.DAL.Repo
{
    public class AuditTrailRepo : Repository<AuditTrail>, IAuditTrailRepo
    {
        private readonly InvoiceDbContext _context;

        public AuditTrailRepo(InvoiceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AuditTrail>> GetAuditLogsAsync()
        {
            return await _context.AuditTrails.OrderByDescending(a => a.Timestamp).ToListAsync();
        }

        public async Task<AuditTrail> AddAuditLogAsync(AuditTrail auditTrail)
        {
            await base.AddAsync(auditTrail);
            return auditTrail;
        }
    }
}
