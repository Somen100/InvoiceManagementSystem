using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

public class Function
{
    public async Task HandleAsync(HttpRequest req, ILogger log)
    {
        log.LogInformation("Function has been invoked");

        var response = new { message = "Hello from .NET Core on Netlify!" };

        await req.HttpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}
