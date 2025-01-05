using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

public class SwaggerFileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in context.ApiDescription.ParameterDescriptions)
        {
            // Check if the parameter comes from FormData (not Body)
            if (parameter.Source.Id == "Form")
            {
                // Check if the parameter is an IFormFile or List<IFormFile>
                if (parameter.ParameterDescriptor.ParameterType == typeof(IFormFile) ||
                    parameter.ParameterDescriptor.ParameterType == typeof(List<IFormFile>))
                {
                    var fileParam = operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name);
                    if (fileParam != null)
                    {
                        fileParam.Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        };
                    }
                }
                else
                {
                    var jsonParam = operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name);
                    if (jsonParam != null)
                    {
                        jsonParam.Schema = new OpenApiSchema
                        {
                            Type = "object"
                        };
                    }
                }
            }
        }
    }
}
