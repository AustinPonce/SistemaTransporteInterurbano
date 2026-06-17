using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = configuration["ApiSettings:ApiKey"];

        if (!context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey) ||
            extractedApiKey != apiKey)
        {
            context.Result = new UnauthorizedObjectResult(
                ApiResponse<object>.Fail("API Key inválida o no proporcionada."));
        }
    }
}
