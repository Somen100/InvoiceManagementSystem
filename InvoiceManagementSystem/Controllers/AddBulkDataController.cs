using InvoiceMgmt.API.DTO;
using InvoiceMgmt.BAL.IService.BulkUploads;
using InvoiceMgmt.Models;
using InvoiceMgmt.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

[Route("api/[controller]")]
[ApiController]
public class AddBulkDataController : ControllerBase
{
    private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
        .WriteTo.File("logs/bulk-Data.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    private readonly IBulkUploadsCustomersService _service;

    public AddBulkDataController(IBulkUploadsCustomersService service)
    {
        _service = service;
    }

    // This is the public endpoint that clients will call
    [HttpPost("upload-bulk-customers")]
    public async Task<IActionResult> UploadBulkCustomers(IFormFile csvFile)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (!csvFile.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase) &&
        !csvFile.ContentType.Equals("application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase) &&
        !csvFile.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file format. Please upload a CSV or Excel file.");
        }


        try
        {
            // Call the helper method to parse the CSV file
            
          //   var customers = await ParseCsvToCustomersAsync(csvFile);
            var customers = await ParseCsvToCustomers(csvFile);

            await _service.AddBulkCustomersAsync(customers);
            return Ok($"{customers.Count} customers successfully uploaded.");
        }
        catch (FormatException ex)
        {
            _logger.Error(ex, "CSV format error.");
            return BadRequest("Invalid CSV format. Ensure the file has correct headers and data.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error uploading bulk customers.");
            return StatusCode(500, "An internal server error occurred while processing the file.");
        }
    }

    // This is the private helper method for parsing the CSV file
    private async Task<List<CustomerBulkDTO>> ParseCsvToCustomersAsync(IFormFile csvFile)
    {
        var customers = new List<CustomerBulkDTO>();

        using (var reader = new StreamReader(csvFile.OpenReadStream()))
        {
            var header = (await reader.ReadLineAsync())?.Split(',');
            if (header == null || header.Length < 5)
            {
                throw new FormatException("Invalid CSV format. Missing required headers.");
            }

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                var fields = line?.Split(',');

                //if (fields == null || fields.Length != header.Length)
                //{
                //    continue; // Skip malformed rows
                //}

                try
                {
                    customers.Add(new CustomerBulkDTO
                    {
                        Name = fields[1],
                        Email = fields[2],
                        PhoneNumber = fields[3],
                        Address = fields[4],
                        IsActive = bool.TryParse(fields[5].Trim('\"'), out var isActive) ? isActive : false
                    });
                }
                catch (Exception ex)
                {
                    _logger.Warning($"Error parsing row: {line}. Exception: {ex.Message}");
                }
            }
        }

        return customers;
    }

    // using csv helper nuget package to split headers automatically:
    private async Task<List<CustomerBulkDTO>> ParseCsvToCustomers(IFormFile csvFile)
    {
        var customers = new List<CustomerBulkDTO>();

        using (var reader = new StreamReader(csvFile.OpenReadStream()))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            // Read the records and map them to Customer objects
            var records = csv.GetRecords<CustomerBulkDTO>().ToList();

            foreach (var record in records)
            {
                // Convert CustomerBulkDTO to Customer (if needed)
                var customer = new CustomerBulkDTO
                {
                    Name = record.Name,
                    Email = record.Email,
                    PhoneNumber = record.PhoneNumber,
                    Address = record.Address,
                    IsActive = true // Assuming all customers are active initially
                };
                customers.Add(customer);
            }
        }

        return customers;
    }

}
