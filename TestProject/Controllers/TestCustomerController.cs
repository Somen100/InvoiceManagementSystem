using InvoiceMgmt.API.Controllers;
using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestProject.MockData;
using Xunit;

namespace TestProject.Controllers
{
    public class TestCustomerController
    {
        [Fact]
        public async Task GetAllCustomers_ShouldReturn200Status()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();

            mockCustomerService.Setup(c => c.GetAllCustomersAsync())
                               .ReturnsAsync(CustomerMockData.GetMockData());

            var controller = new CustomerController(mockCustomerService.Object);

            // Act
            var result = await controller.GetAllCustomers();

            // Assert
        
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Customer>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okResult.StatusCode); // Verify status code

            var customers = Assert.IsType<List<Customer>>(okResult.Value);
            Assert.Equal(2, customers.Count); // Verify the number of customers

        }
    }
}
