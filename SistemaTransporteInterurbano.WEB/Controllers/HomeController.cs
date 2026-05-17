using Microsoft.AspNetCore.Mvc;
using SistemaTransporteInterurbano.BL.Helpers;

namespace SistemaTransporteInterurbano.WEB.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var hash =
            HashHelper.GenerarHash(
                "123456789");

        return Content(hash);
    }
}

