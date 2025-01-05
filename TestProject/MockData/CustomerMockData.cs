using InvoiceMgmt.Models;
using System.Collections.Generic;

namespace TestProject.MockData
{
    public class CustomerMockData
    {
        public static List<Customer> GetMockData()
        {
            return new List<Customer>
            {
                new Customer
                {
                    CustomerId = 107,
                    Name = "Matthew Gilbert",
                    Email = "brian62@gmail.com",
                    PhoneNumber = "(946)905-3928x5",
                    Address = "414 Pamela Hill\nNorth Roberttown, IN 48478",
                    Invoices = null,
                    IsActive = true
                },
                new Customer
                {
                    CustomerId = 108,
                    Name = "James Miller",
                    Email = "christinehernandez@yahoo.com",
                    PhoneNumber = "859-941-7504x26",
                    Address = "0315 Sanchez Plains Apt. 845\nSouth Kimstad, AL 89471",
                    Invoices = null,
                    IsActive = true
                },
            };
        }
    }
}
