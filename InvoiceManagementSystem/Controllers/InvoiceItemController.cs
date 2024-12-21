using InvoiceMgmt.API.DTO;
using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InvoiceMgmt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceItemController : ControllerBase
    {
        private readonly IInvoiceItemService _invoiceItemService;
        private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
            .WriteTo.File("logs/invoiceitem-log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        public InvoiceItemController(IInvoiceItemService invoiceItemService)
        {
            _invoiceItemService = invoiceItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceItems()
        {
            try
            {
                var invoiceItems = await _invoiceItemService.GetInvoiceItemsAsync();
                return Ok(invoiceItems);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in GetInvoiceItems: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceItem(int id)
        {
            try
            {
                var invoiceItem = await _invoiceItemService.GetInvoiceItemByIdAsync(id);
                if (invoiceItem == null)
                {
                    return NotFound();
                }
                return Ok(invoiceItem);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in GetInvoiceItem: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("Invoice/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceItemsByInvoiceId(int invoiceId)
        {
            try
            {
                var invoiceItems = await _invoiceItemService.GetInvoiceItemsByInvoiceIdAsync(invoiceId);
                return Ok(invoiceItems);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in GetInvoiceItemsByInvoiceId: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoiceItem([FromBody] InvoiceItemCreateDTO invoiceItemCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var invoiceItem = new InvoiceItem()
                    {
                        ProductId = invoiceItemCreateDTO.ProductId,
                        Quantity = invoiceItemCreateDTO.Quantity,
                        IsActive = true
                    };

                    var createdInvoiceItem = await _invoiceItemService.CreateInvoiceItemAsync(invoiceItem);
                    return CreatedAtAction(nameof(GetInvoiceItemsByInvoiceId), new { invoiceId = createdInvoiceItem.InvoiceId }, createdInvoiceItem);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in CreateInvoiceItem: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoiceItem(int id, [FromBody] InvoiceItem invoiceItem)
        {
            try
            {
                if (invoiceItem == null || invoiceItem.InvoiceItemId != id)
                {
                    return BadRequest("Invalid invoice item data.");
                }

                var updatedInvoiceItem = await _invoiceItemService.UpdateInvoiceItemAsync(invoiceItem);
                if (updatedInvoiceItem == null)
                {
                    return NotFound();
                }

                return Ok(updatedInvoiceItem);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in UpdateInvoiceItem: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceItem(int id)
        {
            try
            {
                var invoiceItem = await _invoiceItemService.GetInvoiceItemByIdAsync(id);
                if (invoiceItem == null)
                {
                    return NotFound();
                }

                await _invoiceItemService.DeleteInvoiceItemAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in DeleteInvoiceItem: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
