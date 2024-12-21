using InvoiceMgmt.API.DTO;
using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InvoiceMgmt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
           .WriteTo.File("logs/customer-log.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                Log.Error($"Error in GetAllCustomers: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                Log.Error($"Error in GetCustomerById: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CustomerCreateDto customerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("CreateCustomer - Invalid ModelState: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var customer = new Customer()
                {
                    Name = customerDto.Name,
                    Email = customerDto.Email,
                    PhoneNumber = customerDto.PhoneNumber,
                    Address = customerDto.Address,
                    IsActive = true,
                };

                await _customerService.AddCustomerAsync(customer);

                var createdCustomer = await _customerService.GetCustomerByIdAsync(customer.CustomerId);
                if (createdCustomer == null)
                {
                    throw new Exception($"Customer with ID {customer.CustomerId} not found.");
                }

                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId }, createdCustomer);
            }
            catch (Exception ex)
            {
                Log.Error($"Error in CreateCustomer: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (id != customer.CustomerId)
                {
                    return BadRequest();
                }

                await _customerService.UpdateCustomerAsync(id, customer);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in UpdateCustomer: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error($"Error in DeleteCustomer: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
