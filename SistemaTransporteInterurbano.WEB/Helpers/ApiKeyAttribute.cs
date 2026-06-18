using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaTransporteInterurbano.WEB.Models;

namespace SistemaTransporteInterurbano.WEB.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ClaveApiAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = configuration["ApiSettings:ApiKey"];

        if (!context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var extraida) ||
            extraida != apiKey)
        {
            context.Result = new UnauthorizedObjectResult(
                ApiRespuesta<object>.Error("Clave de API inválida o no proporcionada."));
        }
    }
}
