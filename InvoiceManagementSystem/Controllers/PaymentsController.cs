using Microsoft.AspNetCore.Mvc;
using Stripe;

using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace InvoiceMgmt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly string _stripeSecretKey;

        // Constructor to inject configuration
        public PaymentsController(IConfiguration configuration)
        {
            _stripeSecretKey = configuration["Stripe:SecretKey"];
        }

        [HttpPost("CreatePaymentIntent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentRequest paymentRequest)
        {
            // Initialize Stripe with your secret key
            StripeConfiguration.ApiKey = _stripeSecretKey;

            var options = new PaymentIntentCreateOptions
            {
                Amount = paymentRequest.Amount,  // Amount in cents
                Currency = "usd",  // You can change this to another currency
                PaymentMethodTypes = new List<string> { "card" },
            };

            // Create the PaymentMethod with billing details, including postal code
            var paymentMethodOptions = new PaymentMethodCreateOptions
            {
                BillingDetails = new PaymentMethodBillingDetailsOptions
                {
                    Address = new AddressOptions
                    {
                        PostalCode = paymentRequest.PostalCode,  // Include postal code
                    },
                },
            };

            // You may want to create a PaymentMethod here to attach to the PaymentIntent
            var paymentMethodService = new PaymentMethodService();
            var paymentMethod = await paymentMethodService.CreateAsync(paymentMethodOptions);

            options.PaymentMethod = paymentMethod.Id;

            var service = new PaymentIntentService();
            PaymentIntent intent;

            try
            {
                // Create the payment intent
                intent = await service.CreateAsync(options);
            }
            catch (StripeException e)
            {
                // Return a BadRequest if Stripe API throws an error
                return BadRequest(new { error = e.Message });
            }

            // Return the client secret to the front-end to complete the payment
            return Ok(new { clientSecret = intent.ClientSecret });
        }
        [HttpPost("complete")]
        public async Task<IActionResult> CompletePayment([FromBody] PaymentCompletionRequest paymentCompletionRequest)
        {
            // Initialize Stripe with your secret key
            StripeConfiguration.ApiKey = _stripeSecretKey;

            var service = new PaymentIntentService();
            PaymentIntent intent;

            try
            {
                // Confirm the payment intent with the payment method ID
                intent = await service.ConfirmAsync(paymentCompletionRequest.PaymentIntentId, new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentCompletionRequest.PaymentMethodId
                });
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }

            return Ok(new { status = intent.Status, id = intent.Id });
        }


    }

    // Models
    public class PaymentRequest
    {
        public long Amount { get; set; }  // Amount in cents
        public string PostalCode { get; set; }
    }

    public class PaymentCompletionRequest
    {
        public string PaymentIntentId { get; set; }
        public string PaymentMethodId { get; set; }
        public string PostalCode { get; set; }  // Add PostalCode here
    }

}
