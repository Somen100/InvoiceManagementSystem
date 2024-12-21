using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceMgmt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditTrailController : ControllerBase
    {
        public readonly IAuditTrailService _auditTrailService;

        public AuditTrailController(IAuditTrailService auditTrailService)
        {
            _auditTrailService = auditTrailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs()
        {
            try
            {
                var logs = await _auditTrailService.GetAuditLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching audit logs.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuditLog([FromBody] AuditTrail auditTrail)
        {
            if (auditTrail == null)
            {
                return BadRequest("Invalid data.");
            }

            var createdAuditTrail = await _auditTrailService.AddAuditLogAsync(auditTrail);
            return CreatedAtAction(nameof(GetAuditLogs), new { id = createdAuditTrail.AuditId }, createdAuditTrail);
        }
    }
}
