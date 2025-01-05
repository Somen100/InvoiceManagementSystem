using InvoiceMgmt.API.DTO;
using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace InvoiceMgmt.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
           .WriteTo.File("logs/invoice-log.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IAuditTrailService _auditTrailService;
        private readonly IProductService _productService;

        public InvoiceController(IInvoiceService invoiceService, ICustomerService customerService,
          IAuditTrailService auditTrailService, IProductService productService)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _auditTrailService = auditTrailService;
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllInvoice(int pageNumber = 1, int pageSize = 10, string? invoiceNumber = null, string? status = "draft", int? customerId = null)
        {
            try
            {
                var invoiceItems = await _invoiceService.GetAllInvoice(pageNumber, pageSize, invoiceNumber, status, customerId);
                return Ok(invoiceItems);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching all invoices");
                return StatusCode(500, "Internal server error while fetching invoices.");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            try
            {
                var invoiceItem = await _invoiceService.GetInvoiceById(id);
                if (invoiceItem != null)
                {
                    return Ok(invoiceItem);
                }
                return NotFound($"Invoice with ID {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while fetching invoice with ID {id}");
                return StatusCode(500, $"Internal server error while fetching invoice with ID {id}.");
            }
        }

        [AllowAnonymous]
        [HttpDelete]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceById(id);
                if (invoice == null)
                {
                    return NotFound();
                }

                var deletedInvoice = new
                {
                    InvoiceNumber = invoice.InvoiceNumber,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status
                };

                await _invoiceService.DeleteInvoice(id);

                Log.Information("Invoice Deleted: {@Invoice}", deletedInvoice);

                var customer = await _customerService.GetCustomerByIdAsync(invoice.CustomerId);
                if (customer == null)
                {
                    throw new Exception($"Customer with ID {invoice.CustomerId} not found.");
                }

                var auditLog = new AuditTrail
                {
                    TableName = "Invoice",
                    Operation = "Delete",
                    Changes = $"{{ \"InvoiceNumber\": \"{deletedInvoice.InvoiceNumber}\", \"TotalAmount\": \"{deletedInvoice.TotalAmount}\", " +
                              $"\"Status\": \"{deletedInvoice.Status}\" }}",
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = customer.Name
                };

                await _auditTrailService.AddAuditLogAsync(auditLog);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while deleting the invoice");
                return StatusCode(500, "Internal server error while deleting the invoice.");
            }
        }

        //        //front-end:
        //        let formData: FormData = new FormData();
        //        formData.append('File', fileToUpload);
        //formData.append('jsonString', JSON.stringify(myObject));

        ////request using a var of type HttpClient
        //http.post(url, formData);

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateInvoiceAsync( [FromForm] List<IFormFile>? uploadedFiles,[FromForm] string? jsonString)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonString))
                {
                    return BadRequest("The jsonString field is required.");
                }
                jsonString = jsonString.Trim();
                try
                {
                    JToken.Parse(jsonString); // Validates JSON format
                }
                catch (JsonReaderException)
                {
                    return BadRequest("Invalid JSON format.");
                }

                var invoiceDto = JsonConvert.DeserializeObject<InvoiceCreateDTO>(jsonString);

                if (!Enum.TryParse(invoiceDto.Status, true, out InvoiceStatus parsedStatus))
                {
                    return BadRequest($"Invalid status: {invoiceDto.Status}");
                }

                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceDto.InvoiceNumber,
                    CustomerId = invoiceDto.CustomerId,
                    DiscountPercentage = invoiceDto.DiscountPercentage,
                    CreatedAt = invoiceDto.CreatedAt,
                    UpdatedAt = invoiceDto.UpdatedAt,
                    Status = parsedStatus,
                    IsActive = true,
                    InvoiceItems = new List<InvoiceItem>()
                };

                var createdInvoice = await _invoiceService.AddOnlyInvoiceAsync(invoice, uploadedFiles);
                if (createdInvoice == null)
                {
                    return BadRequest("Invoice Not Created");
                }

                foreach (var itemDto in invoiceDto.InvoiceItems)
                {
                    var product = await _productService.GetProductByIdAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        return NotFound($"Product with ID {itemDto.ProductId} does not exist.");
                    }

                    var totalPrice = (product.UnitPrice * itemDto.Quantity) +
                                     ((product.UnitPrice * product.GSTPercentage / 100) * itemDto.Quantity);

                    var invoiceItem = new InvoiceItem
                    {
                        InvoiceId = itemDto.InvoiceId,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        TotalPrice = totalPrice,
                        IsActive = itemDto.IsActive,
                    };

                    createdInvoice.InvoiceItems.Add(invoiceItem);
                }

                createdInvoice.TotalAmount = invoice.InvoiceItems.Sum(i => i.TotalPrice) *
                                             (1 - (invoice.DiscountPercentage / 100));

                await _invoiceService.UpdateOnlyInvoiceAsync(createdInvoice);

                // Audit Log for Invoice Creation
                var customer = await _customerService.GetCustomerByIdAsync(invoice.CustomerId);
                if (customer == null)
                {
                    return NotFound($"Customer with ID {invoice.CustomerId} not found.");
                }

                var auditLog = new AuditTrail
                {
                    TableName = "Invoice",
                    Operation = "Insert",
                    Changes = $"{{ \"InvoiceNumber\": \"{createdInvoice.InvoiceNumber}\", \"TotalAmount\": \"{createdInvoice.TotalAmount}\", " +
                              $"\"Status\": \"{createdInvoice.Status}\" }}",
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = customer.Name
                };

                await _auditTrailService.AddAuditLogAsync(auditLog);
                return CreatedAtAction(nameof(GetInvoiceById), new { id = createdInvoice.InvoiceId }, createdInvoice);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating invoice");
                return StatusCode(500, "Internal server error while creating invoice.");
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceCreateDTO invoiceUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Warning("UpdateInvoice - Invalid ModelState: {@ModelState}, Invoice Update DTO: {@InvoiceUpdateDTO}", ModelState, invoiceUpdateDTO);
                    return BadRequest(ModelState);
                }

                var invoice = await _invoiceService.GetInvoiceById(id);
                if (invoice == null)
                {
                    return NotFound();
                }

                if (!Enum.TryParse(invoiceUpdateDTO.Status, true, out InvoiceStatus parsedStatus))
                {
                    return BadRequest($"Invalid status: {invoiceUpdateDTO.Status}");
                }

                var oldInvoiceState = new
                {
                    InvoiceNumber = invoice.InvoiceNumber,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status,
                    DiscountPercentage = invoice.DiscountPercentage,
                    CustomerId = invoice.CustomerId,
                    CreatedAt = invoice.CreatedAt,
                    UpdatedAt = invoice.UpdatedAt,
                };

                invoice.InvoiceNumber = invoiceUpdateDTO.InvoiceNumber;
                invoice.Status = parsedStatus;
                invoice.CustomerId = invoiceUpdateDTO.CustomerId;
                invoice.UpdatedAt = DateTime.UtcNow;

                var invoiceItems = new List<InvoiceItem>();
                foreach (var itemDto in invoiceUpdateDTO.InvoiceItems)
                {
                    var product = await _productService.GetProductByIdAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        return NotFound($"Product with ID {itemDto.ProductId} does not exist.");
                    }

                    var totalPrice = (product.UnitPrice * itemDto.Quantity) +
                                     ((product.UnitPrice * product.GSTPercentage / 100) * itemDto.Quantity);

                    var invoiceItem = new InvoiceItem
                    {
                        InvoiceId = invoice.InvoiceId,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        TotalPrice = totalPrice,
                        IsActive = itemDto.IsActive,
                    };

                    invoiceItems.Add(invoiceItem);
                }

                await _invoiceService.UpdateInvoice(invoice.InvoiceId, invoice, invoiceItems);
                _logger.Information("Invoice Updated: {@Invoice}", invoice);

                var customer = await _customerService.GetCustomerByIdAsync(invoice.CustomerId);
                if (customer == null)
                {
                    return NotFound($"Customer with ID {invoice.CustomerId} not found.");
                }

                var auditLog = new AuditTrail
                {
                    TableName = "Invoice",
                    Operation = "Update",
                    Changes = $"InvoiceNumber changed from {oldInvoiceState.InvoiceNumber} to {invoice.InvoiceNumber}, " +
                              $"TotalAmount changed from {oldInvoiceState.TotalAmount} to {invoice.TotalAmount}, " +
                              $"Status changed from {oldInvoiceState.Status} to {invoice.Status}",
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = customer.Name
                };

                await _auditTrailService.AddAuditLogAsync(auditLog);

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating invoice");
                return StatusCode(500, "Internal server error while updating invoice.");
            }
        }


        //UploadInvoiceFiles

        //[HttpPost("upload-files/{invoiceId}")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadInvoiceFiles(int invoiceId, [FromForm] ICollection<IFormFile> files)
        //{
        //    try
        //    {
        //        if (files == null || files.Count == 0)
        //        {
        //            return BadRequest("No files uploaded.");
        //        }

        //        // Ensure that the invoice exists before uploading files
        //        var invoice = await _invoiceService.GetInvoiceById(invoiceId);
        //        if (invoice == null)
        //        {
        //            return NotFound($"Invoice with ID {invoiceId} not found.");
        //        }

        //        var uploadedFiles = new List<InvoiceFile>();

        //        foreach (var file in files)
        //        {
        //            // Define the upload folder
        //            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Files", "receipt-images");

        //            // Check if the directory exists; if not, create it
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }

        //            // Generate a unique filename for the uploaded file
        //            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        //            var filePath = Path.Combine(uploadsFolder, fileName);

        //            // Save the file
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            // Create InvoiceFile object to store file data in DB
        //            var invoiceFile = new InvoiceFile
        //            {
        //                InvoiceId = invoiceId,
        //                FileName = fileName,
        //                FilePath = filePath,
        //                UploadDate = DateTime.Now  // Set current date and time
        //            };

        //            uploadedFiles.Add(invoiceFile);
        //        }

        //        // Save all uploaded files to the database
        //        await _context.InvoiceFiles.AddRangeAsync(uploadedFiles);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { Message = "Files uploaded successfully.", Files = uploadedFiles });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log and return an error response
        //        _logger.Error(ex, "Error occurred while uploading invoice files.");
        //        return StatusCode(500, "Internal server error while uploading files.");
        //    }
        //}

    }
}
