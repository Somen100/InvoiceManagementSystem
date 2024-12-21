using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InvoiceMgmt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static readonly Serilog.ILogger _logger = new LoggerConfiguration()
            .WriteTo.File("product-api-log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProduct()
        {
            try
            {
                var products = await _productService.GetAllProductAsync();
                _logger.Information("Successfully retrieved all products.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while retrieving all products.");
                return StatusCode(500, "Internal server error while retrieving products.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    _logger.Warning("Product with ID {ProductId} not found.", id);
                    return NotFound();
                }
                _logger.Information("Successfully retrieved product with ID {ProductId}.", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while retrieving product with ID {ProductId}.", id);
                return StatusCode(500, "Internal server error while retrieving the product.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Warning("AddProduct - Invalid ModelState: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                await _productService.AddProductAsync(product);
                _logger.Information("Successfully added product with ID {ProductId}.", product.ProductId);
                return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while adding product.");
                return StatusCode(500, "Internal server error while adding product.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    _logger.Warning("Product ID mismatch: provided ID {Id}, expected ID {ProductId}.", id, product.ProductId);
                    return BadRequest();
                }

                await _productService.UpdateProductAsync(id, product);
                _logger.Information("Successfully updated product with ID {ProductId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating product with ID {ProductId}.", id);
                return StatusCode(500, "Internal server error while updating product.");
            }
        }

        // removed try-catch block to demostrate global exception handling middleware
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            var product = await _productService.GetProductByIdAsync(id);


            await _productService.DeleteProductAsync(id);
            _logger.Information("Successfully deleted product with ID {ProductId}.", id);
            return NoContent();
        }

    }

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteProduct(int id)
    //{
    //    try
    //    {
    //        var product = await _productService.GetProductByIdAsync(id);
    //        if (product == null)
    //        {
    //            _logger.Warning("Product with ID {ProductId} not found.", id);
    //            return NotFound();
    //        }

    //        await _productService.DeleteProductAsync(id);
    //        _logger.Information("Successfully deleted product with ID {ProductId}.", id);
    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.Error(ex, "Error occurred while deleting product with ID {ProductId}.", id);
    //        return StatusCode(500, "Internal server error while deleting product.");
    //    }
    //}}
}
