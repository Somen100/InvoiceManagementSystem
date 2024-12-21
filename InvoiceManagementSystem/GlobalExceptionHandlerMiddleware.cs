using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace InvoiceMgmt.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;  // Inject IWebHostEnvironment to check if the environment is Development
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception (Log detailed info like stack trace in development)
                if (_env.IsDevelopment())
                {
                    _logger.LogError(ex, "An unexpected error occurred: {Message} \nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                }
                else
                {
                    _logger.LogError(ex, "An unexpected error occurred.");
                }

                // Set the status code and return a generic message
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new ErrorResponse
                {
                    Message = "An unexpected error occurred. Please try again later."
                };

                // In development, include more details in the response body
                if (_env.IsDevelopment())
                {
                    errorResponse.Details = ex.Message;
                }

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
